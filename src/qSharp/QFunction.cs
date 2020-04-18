﻿//
//   Copyright (c) 2011-2014 Exxeleron GmbH
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace qSharp
{
    /// <summary>
    ///     Represents q function.
    ///     Note that the QFunction instances cannot be serialized to IPC protocol.
    /// </summary>
    public class QFunction
    {
        private readonly byte _qTypeCode;

        /// <summary>
        ///     Creates representation of q function with given q type code.
        /// </summary>
        protected QFunction(byte qTypeCode)
        {
            _qTypeCode = qTypeCode;
        }

        /// <summary>
        ///     Retrieve q type code connected with function.
        /// </summary>
        public byte QTypeCode
        {
            get { return _qTypeCode; }
        }

        /// <summary>
        ///     Returns a System.String that represents the current QFunction.
        /// </summary>
        /// <returns>A System.String that represents the current QFunction</returns>
        public override string ToString()
        {
            return "QFunction#" + _qTypeCode + "h";
        }

        /// <summary>
        ///     Creates representation of q function with given q type code.
        /// </summary>
        internal static QFunction Create(byte qTypeCode)
        {
            return new QFunction(qTypeCode);
        }
    }
}