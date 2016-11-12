#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec3.hpp>
#include "VboBase.hpp"

class PositionVbo : public VboBase<glm::vec3>
{
public:
    virtual void Initialize() override
    {
        InitializeToLocation(0);
    }

    virtual void TransferToOpenGl() override
    {
        SendToOpenGl(3, GL_DYNAMIC_DRAW);
    }
};