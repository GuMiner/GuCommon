#pragma once

// Defines a method that will run tests against a class, returning true if all tests pass.
class ITestable
{
public:
    virtual bool Test() = 0;
};