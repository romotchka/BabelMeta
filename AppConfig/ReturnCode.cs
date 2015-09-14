﻿/*
 *  Babel Meta - babelmeta.com
 * 
 *  The present software is licensed according to the MIT licence.
 *  Copyright (c) 2015 Romain Carbou (romain@babelmeta.com)
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE. 
 */

namespace BabelMeta.AppConfig
{
    /// <summary>
    /// ReturnCodes qualifies the output codes returned by functions wherever required in the solution.
    /// Please aim at keeping the running naming convention.
    /// </summary>
    public enum ReturnCode
    {
        Ok,

        ModulesImportDefaultParseEmptyStream,
        ModulesImportDefaultParseUnknownFormat,
        ModulesImportDefaultParseInvalidWorkbook,

        ModulesExportCatalogContextNotInitialized,

        ModulesExportFugaXmlGenerateNullFolderName,

    }

    public static class BabelMetaConfig
    {
        public const int RecordingYearLowerBound = 1900;
        public const int RecordingYearUpperBound = 2100;
    }
}
