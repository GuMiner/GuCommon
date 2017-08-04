#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec3.hpp>
#include "VboBase.hpp"

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
        InitializeToLocation(3);
    }

    virtual void TransferToOpenGl() override
    {
        SendToOpenGl(3, usageType);
    }
};