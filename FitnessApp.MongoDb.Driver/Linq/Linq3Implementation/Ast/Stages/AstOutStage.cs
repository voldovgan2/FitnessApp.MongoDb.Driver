﻿/* Copyright 2010-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using MongoDB.Bson;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Linq.Linq3Implementation.Ast.Visitors;

namespace MongoDB.Driver.Linq.Linq3Implementation.Ast.Stages
{
    internal sealed class AstOutStage : AstStage
    {
        private readonly string _outputCollection;
        private readonly string _outputDatabase;

        public AstOutStage(string outputDatabase, string outputCollection)
        {
            _outputDatabase = outputDatabase;
            _outputCollection = Ensure.IsNotNull(outputCollection, nameof(outputCollection));
        }

        public override AstNodeType NodeType => AstNodeType.OutStage;
        public string OutputCollection => _outputCollection;
        public string OutputDatabase => _outputDatabase;

        public override AstNode Accept(AstNodeVisitor visitor)
        {
            return visitor.VisitOutStage(this);
        }

        public override BsonValue Render()
        {
            return new BsonDocument("$out", RenderOutput());
        }

        private BsonValue RenderOutput()
        {
            return
                _outputDatabase == null ?
                    BsonString.Create(_outputCollection) :
                    new BsonDocument { { "db", _outputDatabase }, { "coll", _outputCollection } };
        }
    }
}
