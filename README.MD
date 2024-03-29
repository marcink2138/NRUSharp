﻿# NRU-Sharp

NRU-Sharp is a discrete event simulator of the listen before talk (LBT) channel access method for frame based equipment (FBE) of the ETSI EN 301 893 standard. The simulator is written in C# using [SimSharp](https://github.com/heal-research/SimSharp), a port of the [SimPy](https://gitlab.com/team-simpy/simpy) discrete event simulation library. The simulator implements not only the standard FBE rules corresponding to the ETSI standardization document, but also introduces the following FBE variants from the literature:

- Fixed-muting FBE
- Random-muting FBE
- Floating FBE
- Deterministic-backoff FBE
- Enhanced FBE
- Greedy-enhanced FBE
- Backoff and idle time reduction FBE

A performance analysis of these variants is available in 

- Karcz, Marcin, and Szymon Szott. "Channel Access in Wireless Smart Grid Networks Operating under ETSI Frame-Based Equipment Rules." Energies 17, no. 1 (2023): 153. [[DOI 10.3390/en17010153](https://doi.org/10.3390/en17010153)] [[PDF](https://www.mdpi.com/1996-1073/17/1/153/pdf?version=1703751170)]

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

## Project dependencies
- [MathNet.Numerics](https://numerics.mathdotnet.com/) 5.0.0
- [MersenneTwister](https://www.nuget.org/packages/MersenneTwister/1.0.6) 1.0.6
- [Microsoft.Data.Analysis](https://www.nuget.org/packages/Microsoft.Data.Analysis/0.20.1) 0.20.1
- [NLog](https://nlog-project.org/) 5.2.3
- [SimSharp](https://www.nuget.org/packages/SimSharp/) 3.4.2

## How to reference NRUSharp?

```
@article{karcz2023channel,
  title={Channel Access in Wireless Smart Grid Networks Operating under ETSI Frame-Based Equipment Rules},
  author={Karcz, Marcin and Szott, Szymon},
  journal={Energies},
  volume={17},
  number={1},
  pages={153},
  year={2023},
  publisher={MDPI},
  doi={10.3390/en17010153}
}
```
