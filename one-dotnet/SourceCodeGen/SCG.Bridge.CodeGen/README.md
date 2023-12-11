# Overview

Generated files will be written to Generated folder under cross package Runtime Scripts. Since this generation will write file during compile code gen, sometimes, it needs several times to write the file successfully as the compilation once interrupted by compiling error, the source code won't be able to be generated.

Due to this, these generated files are source controlled to avoid compile time failure when the project checked out for build.
