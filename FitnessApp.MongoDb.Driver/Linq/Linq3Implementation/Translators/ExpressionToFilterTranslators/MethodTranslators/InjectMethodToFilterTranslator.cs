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

using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Linq.Linq3Implementation.Ast.Filters;
using MongoDB.Driver.Linq.Linq3Implementation.ExtensionMethods;
using MongoDB.Driver.Linq.Linq3Implementation.Misc;
using MongoDB.Driver.Linq.Linq3Implementation.Reflection;

namespace MongoDB.Driver.Linq.Linq3Implementation.Translators.ExpressionToFilterTranslators.MethodTranslators
{
    internal static class InjectMethodToFilterTranslator
    {
        // public static methods
        public static AstFilter Translate(TranslationContext context, MethodCallExpression expression)
        {
            var method = expression.Method;
            var arguments = expression.Arguments;

            if (method.Is(LinqExtensionsMethod.Inject))
            {
                var filterExpression = arguments[0];
                var filterDefinition = filterExpression.GetConstantValue<object>(expression);
                var filterDefinitionType = filterDefinition.GetType(); // we KNOW it's a FilterDefinition<TDocument> because of the Inject method signature
                var documentType = filterDefinitionType.GetGenericArguments()[0];
                var serializerRegistry = BsonSerializer.SerializerRegistry;
                var documentSerializer = serializerRegistry.GetSerializer(documentType); // TODO: is this the right serializer?
                var renderMethodArgumentTypes = new[]
                {
                    typeof(IBsonSerializer<>).MakeGenericType(documentType),
                    typeof(IBsonSerializerRegistry),
                    typeof(LinqProvider)
                };
                var renderMethod = filterDefinitionType.GetMethod("Render", renderMethodArgumentTypes);
                var renderedFilter = (BsonDocument)renderMethod.Invoke(filterDefinition, new object[] { documentSerializer, serializerRegistry, LinqProvider.V3 });
                return AstFilter.Raw(renderedFilter);
            }

            throw new ExpressionNotSupportedException(expression);
        }
    }
}
