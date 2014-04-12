﻿using BitSharp.Common;
using BitSharp.Common.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BitSharp.Core.Domain;
using BitSharp.Core.Storage;
using BitSharp.Core;

namespace BitSharp.Esent
{
    public class ChainedBlockStorage : EsentDataStorage<ChainedBlock>, IChainedBlockStorage
    {
        public ChainedBlockStorage(string baseDirectory)
            : base(baseDirectory, "chainedBlocks",
                chainedBlock => DataEncoder.EncodeChainedBlock(chainedBlock),
                (blockHash, bytes) => DataEncoder.DecodeChainedBlock(bytes))
        { }
    }
}