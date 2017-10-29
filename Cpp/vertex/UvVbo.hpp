#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec2.hpp>
#include "VboBase.hpp"

class UvVbo : public VboBase<glm::vec2>
{
    GLenum usageType;

public:
    UvVbo(GLenum usageType = GL_DYNAMIC_DRAW)
        : usageType(usageType)
    {

    }

    virtual void SetupOpenGlBuffers() override
    {
        InitializeToLocation(2);
    }

    virtual void TransferToOpenGl() override
    {
        SendToOpenGl(2, usageType);
    }
};