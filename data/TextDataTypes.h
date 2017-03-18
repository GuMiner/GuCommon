#pragma once
#include <map>
#include <GL/glew.h>
#include <glm\mat4x4.hpp>
#include "vertex\ColorVbo.hpp"
#include "vertex\PositionVbo.hpp"
#include "vertex\UvVbo.hpp"

// Holds all of the bitmap data necessary for a character in the font.
struct CharInfo
{
    // The texture X and Y locations where this character is stored.
    int textureX;
    int textureY;

    // Amount that the unscaled parameters should be scaled by for this font.
    float scale;

    // The character bitmap, in 8 bits-per-pixel format.
    unsigned char *characterBitmap;

    // Horizontal amount to advance for this character (unscaled)
    int advanceWidth;

    // Offset from the current horizontal position to the left edge of the character (unscaled)
    int leftSideBearing;

    // Character size and offsets.
    int width;
    int height;
    int xOffset;
    int yOffset;
};

// Holds all of the cached bitmaps for the various character sizes of a character.
struct TextInfo
{
    // Maps pixel size of a character into the related character info sizes.
    std::map<int, CharInfo> characterSizes;

    // Font ascent (unscaled)
    int ascent;
};

// Holds all the OpenGL information necessary to render a sentence
struct SentenceInfo
{
    GLuint vao;
    PositionVbo positions;
    ColorVbo colors;
    UvVbo uvs;

    GLsizei characterCount;
    GLint *characterStartIndices;
    GLsizei *characterVertexCounts;

    // Dimensions in terms of pixels fo texture consumed by the sentence.
    glm::vec2 sententenceSize;

    // Font size and color of the sentence.
    int fontSize;
    glm::vec3 textColor;

    SentenceInfo()
        : characterCount(0), characterStartIndices(nullptr), characterVertexCounts(nullptr)
    {
    }
};
