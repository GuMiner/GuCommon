#pragma once
#include <vector>
#include <GL\glew.h>
#include <glm\vec2.hpp>
#include <glm\vec3.hpp>
#include "ColorVbo.hpp"
#include "PositionVbo.hpp"
#include "UvVbo.hpp"

class VboHelper
{
public:
    static void AddColorTextureVertex(glm::vec3 position, glm::vec3 color, glm::vec2 uvPos,
        PositionVbo& positionVbo, ColorVbo& colorVbo, UvVbo& uvVbo)
    {
        positionVbo.vertices.push_back(position);
        colorVbo.vertices.push_back(color);
        uvVbo.vertices.push_back(uvPos);
    }
};