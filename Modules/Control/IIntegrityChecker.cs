/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataConverter.Modules.Control
{
    public interface IIntegrityChecker
    {
        bool CheckRedundantKeys();

        bool CheckReferentialIntegrity();
    }
}
