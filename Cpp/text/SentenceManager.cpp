#include <cstring>
#include <fstream>
#include <vector>
#include <algorithm>
#include "logging\Logger.h"
#include "vertex\VboHelper.hpp"
#include "SentenceManager.h"

SentenceManager::SentenceManager()
{
    loadedFontFile = nullptr;
    usedWidth = 0;
    usedHeight = 0;
    lastMaxHeight = 0;
    nextSentenceId = 0;
}

bool SentenceManager::LoadFont(ShaderFactory *shaderManager, const char *fontName)
{
    /// Load in our shader for the font.
    if (!shaderManager->CreateShaderProgram("../gucommon/text/sentenceRender", &fontShader))
    {
        Logger::LogError("Bad sentence shader!");
        return false;
    }

    mvLocation = glGetUniformLocation(fontShader, "mv_matrix");
    projLocation = glGetUniformLocation(fontShader, "proj_matrix");
    fontImageLocation = glGetUniformLocation(fontShader, "fontimage");

    /// Load in the font file
    std::ifstream file(fontName, std::ios::binary | std::ios::ate);
    if (!file)
    {
        Logger::LogError("Couldn't read the font file!");
        return false;
    }

    // We started at the *end* of the file, so we know how long it is! Use that to allocate data and read in the entire file.
    std::ifstream::pos_type fileLength = file.tellg();
    file.seekg(0, std::ios::beg);

    std::vector<char> buffer((unsigned int)fileLength);
    file.read(&buffer[0], fileLength);
    file.close();

    // Transfer over the file to our permament buffer.
    loadedFontFile = new unsigned char[(unsigned int)fileLength];
    memcpy(loadedFontFile, &buffer[0], (size_t)fileLength);

    // Initialize the font and generic OpenGL info.
    stbtt_InitFont(&fontInfo, loadedFontFile, 0);

    // Create a new texture for the image.
    glGenTextures(1, &fontTexture);

    // Bind the texture and prepare to send in image data.
    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, fontTexture);

    // If you need more pixels for the font, make a PR and improve this section of code.
    width = 1024;
    height = 1024;
    glTexStorage2D(GL_TEXTURE_2D, 1, GL_RGBA8, width, height);

    return true;
}

// Adds the specified font to the font texture, loading the character position information into the provided structure.
void SentenceManager::AddToFontTexture(CharInfo& charInfo)
{
    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, fontTexture);

    // Move down a row if we need to.
    if (usedWidth + charInfo.width > width)
    {
        usedWidth = 0;
        usedHeight += lastMaxHeight;
        lastMaxHeight = 0;
    }

    // Store the image and move around our current position.
    glTexSubImage2D(GL_TEXTURE_2D, 0, usedWidth, usedHeight, charInfo.width, charInfo.height, GL_RGBA, GL_UNSIGNED_BYTE, charInfo.characterBitmap);
    charInfo.textureX = usedWidth;
    charInfo.textureY = usedHeight;

    usedWidth += charInfo.width + 1; // 1 px buffer space
    lastMaxHeight = (int)std::max((float)lastMaxHeight, (float)(charInfo.height + 1));
}

// Returns the character info pertaining to the specified font pixel height and character
CharInfo& SentenceManager::GetCharacterInfo(int fontPixelHeight, int character)
{
    if (fontData.count(character) == 0)
    {
        // There is no characters at all loaded of the specific type, perform a load of generic text information.
        TextInfo textInfo;
        stbtt_GetFontVMetrics(&fontInfo, &textInfo.ascent, 0, 0);

        fontData[character] = textInfo;
    }

    if (fontData[character].characterSizes.count(fontPixelHeight) == 0)
    {
        // We need to add to the mapping of character sizes this new character size.
        CharInfo charInfo;
        charInfo.scale = stbtt_ScaleForPixelHeight(&fontInfo, (float)fontPixelHeight);
        stbtt_GetCodepointHMetrics(&fontInfo, character, &charInfo.advanceWidth, &charInfo.leftSideBearing);
        charInfo.characterBitmap = stbtt_GetCodepointBitmap(&fontInfo, 0, charInfo.scale, character, &charInfo.width, &charInfo.height, &charInfo.xOffset, &charInfo.yOffset);

        // Make this an RGBA bitmap image.
        unsigned char* extraStorage = new unsigned char[charInfo.width*charInfo.height * 4];
        for (int j = 0; j < charInfo.height; j++)
        {
            for (int i = 0; i < charInfo.width; i++)
            {
                extraStorage[(i + j*charInfo.width) * 4 + 0] = charInfo.characterBitmap[i + j*charInfo.width];
                extraStorage[(i + j*charInfo.width) * 4 + 1] = charInfo.characterBitmap[i + j*charInfo.width];
                extraStorage[(i + j*charInfo.width) * 4 + 2] = charInfo.characterBitmap[i + j*charInfo.width];
                extraStorage[(i + j*charInfo.width) * 4 + 3] = charInfo.characterBitmap[i + j*charInfo.width];
            }
        }

        stbtt_FreeBitmap(charInfo.characterBitmap, nullptr);
        charInfo.characterBitmap = extraStorage;

        AddToFontTexture(charInfo);

        fontData[character].characterSizes[fontPixelHeight] = charInfo;
    }

    return fontData[character].characterSizes[fontPixelHeight];
}

int SentenceManager::GetSentenceVertexCount(const std::string& sentence)
{
    // Note that we 'render' space, tab, etc.
    return sentence.size() * VerticesPerChar;
}

// Given a sentence, allocates the vertexes corresponding to the sentence.
// The vertexes start at (0, 0, 0) and go in the X-direction, with 1 unit == pixelHeight.
void SentenceManager::AllocateSentenceVertices(const std::string& sentence, int pixelHeight, glm::vec3 textColor, SentenceInfo& sentenceInfo)
{
    float lastZPos = 0.0f;
    float lastXPos = 0.0f;

    // We use a constant scale so that 12-pt font is a reasonable size.
    float vertScale = 0.1f * (float)pixelHeight / 12.0f;

    // Render out all our characters
    int maxHeight = std::numeric_limits<int>::min();
    for (int i = 0; i < (int)sentence.size(); i++)
    {
        // Save the maximum height to generate borders.
        CharInfo& charInfo = GetCharacterInfo(pixelHeight, sentence[i]);
        if (charInfo.height > maxHeight)
        {
            maxHeight = charInfo.height;
        }

        // Character vertex positions.
        float effectiveWidth = vertScale * (float)charInfo.width;
        float advanceWidth = vertScale * (float)charInfo.advanceWidth * (float)charInfo.scale;
        float leftSideBearing = vertScale * (float)charInfo.leftSideBearing * (float)charInfo.scale;

        float xStart = lastXPos + leftSideBearing;
        float xDepth = effectiveWidth + xStart;

        float yStart = vertScale * (float)charInfo.yOffset;
        float yDepth = vertScale * (float)charInfo.height + yStart;

        // Character texture vertex positions.
        float textureX = (float)charInfo.textureX / (float)width;
        float textureY = (float)charInfo.textureY / (float)height;
        float textureXEnd = (float)(charInfo.textureX + charInfo.width) / (float)width;
        float textureYEnd = (float)(charInfo.textureY + charInfo.height) / (float)height;

        // Triangle fan. First position is at start, then +x, +x+y, +y
        VboHelper::AddColorTextureVertex(glm::vec3(xStart, -yStart, lastZPos), glm::vec3(textColor.x, textColor.y, textColor.z), glm::vec2(textureX, textureY), 
            sentenceInfo.positions, sentenceInfo.colors, sentenceInfo.uvs);
        VboHelper::AddColorTextureVertex(glm::vec3(xStart, -yDepth, lastZPos), glm::vec3(textColor.x, textColor.y, textColor.z), glm::vec2(textureX, textureYEnd),
            sentenceInfo.positions, sentenceInfo.colors, sentenceInfo.uvs);
        VboHelper::AddColorTextureVertex(glm::vec3(xDepth, -yDepth, lastZPos), glm::vec3(textColor.x, textColor.y, textColor.z), glm::vec2(textureXEnd, textureYEnd),
            sentenceInfo.positions, sentenceInfo.colors, sentenceInfo.uvs);
        VboHelper::AddColorTextureVertex(glm::vec3(xDepth, -yStart, lastZPos), glm::vec3(textColor.x, textColor.y, textColor.z), glm::vec2(textureXEnd, textureY),
            sentenceInfo.positions, sentenceInfo.colors, sentenceInfo.uvs);

        lastXPos += advanceWidth;
    }

    sentenceInfo.sententenceSize = glm::vec2(lastXPos, vertScale * (float)maxHeight);
}

// Simulates the length of a sentence if it was made with the given text and pixel height.
float SentenceManager::SimulateSentenceLength(const std::string& sentence, int pixelHeight)
{
    float vertScale = 1.0f;

    // Scale the vertical scale according to the tallest character.
    int maxHeight;
    for (int i = 0; i < (int)sentence.size(); i++)
    {
        CharInfo& charInfo = GetCharacterInfo(pixelHeight, sentence[i]);
        if (i == 0)
        {
            vertScale = 1.0f / (float)charInfo.height;
            maxHeight = charInfo.height;
        }
        else if (charInfo.height > maxHeight)
        {
            vertScale = 1.0f / (float)charInfo.height;
            maxHeight = charInfo.height;
        }
    }

    // Determine the sentence length by using the advance width.
    float sentenceLength = 0.0f;
    for (int i = 0; i < (int)sentence.size(); i++)
    {
        CharInfo& charInfo = GetCharacterInfo(pixelHeight, sentence[i]);

        float advanceWidth = vertScale * (float)charInfo.advanceWidth * (float)charInfo.scale;
        sentenceLength += advanceWidth;
    }

    return sentenceLength;
}

// Creates a new sentence that can be referenced for drawing.
int SentenceManager::CreateNewSentence()
{
    // Loads in the OpenGL components for this sentence.
    SentenceInfo sentenceInfo = SentenceInfo();
    glGenVertexArrays(1, &sentenceInfo.vao);
    glBindVertexArray(sentenceInfo.vao);

    sentenceInfo.positions.Initialize();
    sentenceInfo.colors.Initialize();
    sentenceInfo.uvs.Initialize();

    sentences[nextSentenceId] = sentenceInfo;
    ++nextSentenceId;

    return nextSentenceId - 1;
}

void SentenceManager::UpdateSentence(int sentenceId, const std::string& sentence)
{
    SentenceInfo& sentenceInfo = sentences[sentenceId];
    UpdateSentence(sentenceId, sentence, sentenceInfo.fontSize, sentenceInfo.textColor);
}

// Updates the graphical components of a sentence so it can be drawn.
void SentenceManager::UpdateSentence(int sentenceId, const std::string& sentence, int pixelHeight, glm::vec3 textColor)
{
    SentenceInfo& sentenceInfo = sentences[sentenceId];

    // Parse our the text textures
    sentenceInfo.characterCount = sentence.size();
    AllocateSentenceVertices(sentence, pixelHeight, textColor, sentenceInfo);

    // Send that data to OpenGL
    glBindVertexArray(sentenceInfo.vao);
    sentenceInfo.positions.TransferToOpenGl();
    sentenceInfo.colors.TransferToOpenGl();
    sentenceInfo.uvs.TransferToOpenGl();
    sentenceInfo.positions.vertices.clear();
    sentenceInfo.colors.vertices.clear();
    sentenceInfo.uvs.vertices.clear();

    // Update our character indices and vertex counts so we can do a multi-element drawing scheme.
    ClearCharacterData(sentenceInfo);

    GLint* startingElements = new GLint[sentenceInfo.characterCount];
    GLsizei* elementCounts = new GLsizei[sentenceInfo.characterCount];
    for (int i = 0; i < sentenceInfo.characterCount; i++)
    {
        startingElements[i] = i * VerticesPerChar;
        elementCounts[i] = VerticesPerChar;
    }

    sentenceInfo.characterStartIndices = startingElements;
    sentenceInfo.characterVertexCounts = elementCounts;

    sentenceInfo.fontSize = pixelHeight;
    sentenceInfo.textColor = textColor;
}

// Renders the specified sentence.
void SentenceManager::RenderSentence(int sentenceId, const glm::mat4& perpective, const glm::mat4& mvMatrix)
{
    const SentenceInfo sentenceInfo = sentences[sentenceId];
    if (sentenceInfo.characterStartIndices == nullptr)
    {
        // No sentence data to render, exit early.
        return;
    }

    glUseProgram(fontShader);
    glUniform1i(fontImageLocation, 0);

    // Bind in the texture and vertices we're using.
    glBindVertexArray(sentenceInfo.vao);

    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, fontTexture);

    glUniformMatrix4fv(projLocation, 1, GL_FALSE, &perpective[0][0]);
    glUniformMatrix4fv(mvLocation, 1, GL_FALSE, &mvMatrix[0][0]);

    // Draw the text
    glMultiDrawArrays(GL_TRIANGLE_FAN, sentenceInfo.characterStartIndices, sentenceInfo.characterVertexCounts, sentenceInfo.characterCount);
}

glm::vec2 SentenceManager::GetSentenceDimensions(int sentenceId)
{
    return sentences[sentenceId].sententenceSize;
}

void SentenceManager::ClearCharacterData(const SentenceInfo& sentenceInfo)
{
    if (sentenceInfo.characterStartIndices != nullptr)
    {
        delete[] sentenceInfo.characterStartIndices;
    }

    if (sentenceInfo.characterVertexCounts != nullptr)
    {
        delete[] sentenceInfo.characterVertexCounts;
    }
}

void SentenceManager::DeleteSentence(SentenceInfo& sentenceInfo)
{
    glDeleteVertexArrays(1, &sentenceInfo.vao);
    sentenceInfo.positions.Deinitialize();
    sentenceInfo.colors.Deinitialize();
    sentenceInfo.uvs.Deinitialize();

    ClearCharacterData(sentenceInfo);
}

void SentenceManager::DeleteSentence(int sentenceId)
{
    SentenceInfo& sentenceInfo = sentences[sentenceId];
    DeleteSentence(sentenceInfo);
    sentences.erase(sentenceId);
}

SentenceManager::~SentenceManager()
{
    // Free all of our loaded OpenGL resources
    for (std::map<int, SentenceInfo>::iterator iterator = sentences.begin(); iterator != sentences.end(); iterator++)
    {
        DeleteSentence(iterator->second);
    }

    // Free all of the loaded font bitmaps at program end.
    for (std::map<int, TextInfo>::iterator iterator = fontData.begin(); iterator != fontData.end(); iterator++)
    {
        std::map<int, CharInfo>& charSizes = iterator->second.characterSizes;
        for (std::map<int, CharInfo>::iterator charIterator = charSizes.begin(); charIterator != charSizes.end(); charIterator++)
        {
            delete[] charIterator->second.characterBitmap;
        }
    }

    // Deletes the loaded font (that stb references)
    if (loadedFontFile != nullptr)
    {
        delete[] loadedFontFile;
    }

    glDeleteTextures(1, &fontTexture);
}
