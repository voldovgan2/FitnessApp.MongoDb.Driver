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

namespace MongoDB.Driver.Linq.Linq3Implementation.Ast.Filters
{
    internal sealed class AstGeoWithinCenterFilterOperation : AstFilterOperation
    {
        private readonly BsonValue _radius;
        private readonly BsonValue _x;
        private readonly BsonValue _y;

        public AstGeoWithinCenterFilterOperation(BsonValue x, BsonValue y, BsonValue radius)
        {
            _x = Ensure.IsNotNull(x, nameof(x));
            _y = Ensure.IsNotNull(y, nameof(y));
            _radius = Ensure.IsNotNull(radius, nameof(radius));
        }

        public override AstNodeType NodeType => AstNodeType.GeoWithinCenterFilterOperation;
        public BsonValue Radius => _radius;
        public BsonValue X => _x;
        public BsonValue Y => _y;

        public override AstNode Accept(AstNodeVisitor visitor)
        {
            return visitor.VisitGeoWithinCenterFilterOperation(this);
        }

        public override BsonValue Render()
        {
            return new BsonDocument("$geoWithin", new BsonDocument("$center", new BsonArray { new BsonArray { _x, _y }, _radius }));
        }
    }
}
