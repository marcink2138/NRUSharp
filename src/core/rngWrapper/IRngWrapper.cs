﻿namespace NRUSharp.core.rngWrapper{
    public interface IRngWrapper{
        public void Init(int seed = 0);
        public int GetInt(int start, int end);
    }
}