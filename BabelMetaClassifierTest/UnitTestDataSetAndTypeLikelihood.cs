/*
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

using System;
using System.Collections.Generic;
using BabelMetaClassifier.Model;
using BabelMetaClassifier.Model.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BabelMetaClassifierTest
{
    [TestClass]
    public class UnitTestDataSetAndTypeLikelihood
    {
        [TestMethod]
        public void TestMethodPopulateDataSet()
        {
            var dataSet = new DataSet();
            dataSet.AddRow(new List<string>
            {
                "EVENEMENT",
                "ANNEE",
                "PAYS",
                "COMMENTAIRE"
            });

            dataSet.AddRow(new List<string>
            {
                "Grand Schisme d'Orient",
                "1054",
                "",
            });

            dataSet.AddRow(new List<string>
            {
                "Marignan",
                "1515",
                "France",
                "Victoire",
            });
            dataSet.AddRow(new List<string>
            {
                "Révolution",
                "1917",
                "Russie",
            });
            dataSet.AddRow(new List<string>
            {
                "Pearl Harbour",
                "1941",
                "Etats-Unis",
                "Attaque japonaise",
            });
            dataSet.AddRow(new List<string>
            {
                "Déclaration Universelle des Droits de l'Homme",
                "1948",
                "",
            });

            var timewindow = new Year
            {
                MinPossibleYear = 1000,
                MinLikelyYear = 1500,
                MaxLikelyYear = 2020,
                MaxPossibleYear = 2100,
            };

            dataSet.DataTypes.Add(timewindow);

            dataSet.InitializeColumnTypeLikelihood(dataSet.ColumnIndexes[0], timewindow); // 0.0
            dataSet.InitializeColumnTypeLikelihood(dataSet.ColumnIndexes[1], timewindow); // 0.81... (Title is not a year)
            dataSet.InitializeColumnTypeLikelihood(dataSet.ColumnIndexes[2], timewindow); // 0.16... (Empty string can be a year)
        }

    }
}
