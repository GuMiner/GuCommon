#pragma once
#include <vector>
#include <GL/glew.h>

// Defines a base vertex type for VBOs. 
template<typename T>
class VboBase
{
public:
    std::vector<T> vertices;

private:
    GLuint buffer;
    GLuint layoutPosition;

protected:

    // Child classes typically implement TransferToOpenGl by implmenting the item count and usage best for that type.
    void SendToOpenGl(GLuint itemCount, GLenum usage)
    {
        glEnableVertexAttribArray(layoutPosition);
        glBindBuffer(GL_ARRAY_BUFFER, buffer);
        glVertexAttribPointer(layoutPosition, itemCount, GL_FLOAT, GL_FALSE, 0, nullptr);

        glBufferData(GL_ARRAY_BUFFER, vertices.size()*sizeof(T), &vertices[0], usage);
    }
    
    // Child classes determine the layout position.
    void InitializeToLocation(GLuint layoutPosition)
    {
        glGenBuffers(1, &buffer);
        layoutPosition = layoutPosition;
    }

public:
    virtual void Initialize() = 0;
    virtual void TransferToOpenGl() = 0;

    void Deinitialize()
    {
        glDeleteBuffers(1, &buffer);
    }
};