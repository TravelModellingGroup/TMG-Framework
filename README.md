![CI](https://github.com/TravelModellingGroup/TMG-Framework/workflows/CI/badge.svg?branch=dev)

# TMG-Framework
The Travel Modelling Group's modules for XTMF2.

Functionality for XTMF2 has been broken up into several repositories:
* [XTMF2](https://github.com/TravelModellingGroup/XTMF2) repository contains the
core functionality for developing and running model systems.
* [TMG.Tasha2](https://github.com/TravelModellingGroup/TMG.Tasha2) contains modules specific to
GTAModel V4+.
* [TMG.EMME](https://github.com/TravelModellingGroup/TMG.EMME) contains the modules
for interacting with INRO's EMME software.  Additionally it contains TMG's TMGToolbox2 for EMME.
* [XTMF2.Web](https://github.com/TravelModellingGroup/XTMF2.Web) provides a web user experience for
operating XTMF2.

## Building TMG-Framework

### Requirements

1. DotNet Core 3.1+ SDK

### Clone the TMG-Framework repository

> git clone https://github.com/TravelModellingGroup/TMG-Framework.git

### Compile from the command line

> dotnet build -c Debug

or

> dotnet build -c Release

and then test with,

> dotnet test

## Main Branches

There are two main branches for TMG-Framework:
* [dev](https://github.com/TravelModellingGroup/TMG-Framework/tree/dev) contains the latest
development build.  This branch takes in all Pull Requests.
* [master](https://github.com/TravelModellingGroup/TMG-Framework/tree/master) contains the
latest stable version of the TMG-Framework.