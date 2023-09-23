# NRU-Sharp

NRU-Sharp is a discrete event simulator of the listen before talk (LBT) channel access method for frame based equipment (FBE) of the ETSI EN 301 893 standard. The simulator is written in C# using [SimSharp](https://github.com/heal-research/SimSharp), a port of the [SimPy](https://gitlab.com/team-simpy/simpy) discrete event simulation library. The simulator implements not only the standard FBE rules corresponding to the ETSI standardization document, but also introduces the following FBE variants from the literature:

- Fixed-muting FBE
- Random-muting FBE
- Floating FBE
- Deterministic-backoff FBE
- Enhanced FBE
- Greedy-enhanced FBE
- Backoff and idle time reduction FBE

Each of the simulation components is designed to be fully configurable and extendable without hardcoded parameters.
The simulator collects two types of metrics:

1. Per node metrics:
    - successful transmissions
    - failed transmissions
    - air time
    - mean channel access delay
2. Aggregated metrics:  
   - channel efficiency
   - Jain's fairness index

## Usage

Configuration examples and usage are included in the [tests](tests) directory. Each variant implementation object is reusable by calling `reset()` after a simulation run, and can be constructed via the associated `builder`. The `ScenarioRunner` class is responsible for running a user test case which is based on `ScenarioMatrix`. Each row in `ScenarioMatrix` describes a particular node object implementation for a particular simulation run (column).
