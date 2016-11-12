#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec2.hpp>
#include "VboBase.hpp"

class UvVbo : public VboBase<glm::vec2>
{
public:
    virtual void Initialize() override
    {
        InitializeToLocation(2);
    }

    virtual void TransferToOpenGl() override
    {
        SendToOpenGl(2, GL_DYNAMIC_DRAW);
    }
};