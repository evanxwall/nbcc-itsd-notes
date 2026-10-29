*npm* stands for **Node Package Manager**. It comes with node.js when you install it. It lets you install, run, and manage different packages (your libraries and tools) in your JavaScript projects. 

**Some Commands:**
- `npm init`:                                       Initializes a node.js project                         
- `npm install packageName`:           Installs the package you specific to your current project.
- `npm uninstall packageName`:       Uninstalls the package you specific to your current project.
- `npm list`:                                       Lists installed packages in the current project.
- `npm outdated`:                               Checks for outdated install packages.
- `npm update`:                                   Updates installed packages.
- `npm audit`:                                     Checks dependencies for known security vulnerabilities.
- `npm help`:                                       Display's npm help menu
- `npm view express version`:         Shows the published version of express.
- `npm cache verify`:                        Verifies npm's local cache.
- `npm ci`:                                           Clean, reproducible installation based on the lockfile

**Some Flags:**
*You don't need to use most of these. Just wrote them down whenever I found a new one while researching.*

Common Flags
- `-y` or `--yes`:                          Automatically answer yes to prompts (useful for `npm init -y`)
- `-g` or `--global`:                    Installs a package globally. 
- `-f` or `--force`:                      Force an operation if npm applies safeguards
- `-h` or `--help`:                        Display help.
- `-v` or `--version`:                  Shows the current version of npm
- `-D` or `--save-dev`:                Adds a development dependency.
- `-S` or `--save`:                        Save as a regular dependency.
- `-O` or `--save-optional`:       Add an optional dependency
- `-E` or `--save-exact`:             Save an exact package version
- `-P` or `--save-prod`:               Save as a production dependency

**Some Useful Install Commands:**
`npm install express`                    - you can also add `hbs` to the end for the handlebars template
`npm install express-session`     - When using session variables, install separately
`npm install hbs`                           - It can also just be installed on it own
`npm install nodemon`
`npm install mongodb`
