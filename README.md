# GuCommon
## Synopsis
-----------
This repository contains common code used throughout my projects. 

## Usage
--------------
The author suggests downloading this code as a submodule and integrating the code directly into your application; the solution is provided only for testing, verification, and to automatically generate the 'Project.txt' and 'Filter.txt' files.

To integrate this code in your own application:

1. Download the dependencies -- put libraries and include files in your project.
2. 'git submodule add https://github.com/GuMiner/GuCommon.git'
3. 'git submodule init'
4. 'git submodule update'
5. Add the code you'd like to consume.

## Dependencies
---------------
If you're including this DLL or using this code directly, you'll also need these dependencies. Place the 'lib' files in the lib directory, 'include' files in the include directory, and DLLs (if applicable) in the consuming project.

* [GLM 0.9.8.1] (http://glm.g-truc.net/0.9.8/index.html)
* [GLFW 3.2.1] (http://www.glfw.org/)
* [GLEW 1.12] (http://glew.sourceforge.net/)
* [STB latest] (https://github.com/nothings/stb)