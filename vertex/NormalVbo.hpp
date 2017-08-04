#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec3.hpp>
#include "VboBase.hpp"

// NOTE: This takes the place of the Color VBO
class NormalVbo : public VboBase<glm::vec3>
{
    GLenum usageType;

public:
    NormalVbo(GLenum usageType = GL_DYNAMIC_DRAW)
        : usageType(usageType)
    {

    }

    virtual void Initialize() override
    {
        InitializeToLocation(1);
    }

    virtual void TransferToOpenGl() override
    {
        SendToOpenGl(3, usageType);
    }
};