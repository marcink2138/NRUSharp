# NRU-Sharp
NRU-Sharp is a discrete event simulator of Frame Based Equipment (FBE) channel access method written in C# with use of SimSharp [1], a SimPy DES library [2] port for C#. The simulator implements not only FBE which principle of operation matches ETSI standardization document but also introduces the following FBE variants:
- Fixed-muting FBE
- Random-muting FBE
- Floating FBE
- Deterministic-backoff FBE
- Enhanced FBE
- Greedy-enhanced FBE
- Backoff and idle time reduction FBE

Each of the simulation components was designed to be fully configurable and extendable without hardcoded parameters.
The simulator collects two types of metrics:

1. Per node metrics:

    - successful transmissions
    - failed transmissions
    - air time
    - mean channel access delay
2. Aggregated metrics:
   
   - channel efficiency
   - Jain's Fairness index

## Usage
The examples of configuration and usage are included in [tests](tests) directory. Each variant implementation object is reusable by calling *reset()* after a simulation run, and can be constructed via the associated *builder*. *ScenarioRunner* class is responsible for running a user test case which is based on *ScenarioMatrix*. Each row in *ScenarioMatrix* describes a particular node object implementation for a particular simulation run (column).

## References

1. [Python SimPy](https://gitlab.com/team-simpy/simpy)
2. [C# SimSharp](https://github.com/heal-research/SimSharp)