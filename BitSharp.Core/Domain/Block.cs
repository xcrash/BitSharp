﻿using BitSharp.Common;
using BitSharp.Common.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BitSharp.Core.Domain
{
    public class Block
    {
        private readonly BlockHeader _header;
        private readonly ImmutableArray<Transaction> _transactions;

        public Block(BlockHeader header, ImmutableArray<Transaction> transactions)
        {
            this._header = header;
            this._transactions = transactions;
        }

        public UInt256 Hash { get { return this.Header.Hash; } }

        public BlockHeader Header { get { return this._header; } }

        public ImmutableArray<Transaction> Transactions { get { return this._transactions; } }

        public Block With(BlockHeader Header = null, ImmutableArray<Transaction>? Transactions = null)
        {
            return new Block
            (
                Header ?? this.Header,
                Transactions ?? this.Transactions
            );
        }
    }
}
