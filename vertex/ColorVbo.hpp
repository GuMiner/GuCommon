#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec3.hpp>
#include "VboBase.hpp"

class ColorVbo : public VboBase<glm::vec3>
{
    GLenum usageType;

public:
    ColorVbo(GLenum usageType = GL_DYNAMIC_DRAW)
        : usageType(usageType)
    {

    }

    virtual void SetupOpenGlBuffers() override
    {
        InitializeToLocation(1);
    }

    virtual void TransferToOpenGl() override
    {
        SendToOpenGl(3, usageType);
    }
};