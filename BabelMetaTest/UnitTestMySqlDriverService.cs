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

using System.Diagnostics;
using BabelMeta;
using BabelMeta.Model;
using BabelMeta.Services.DbDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BabelMetaTest
{
    [TestClass]
    public class UnitTestMySqlDriverService
    {
        private MainFormViewModel _viewModel;

        [TestMethod]
        public void TestInsertMany()
        {
            _viewModel = new MainFormViewModel();

            // TODO: Load sample Catalog here.

            MySqlDriverService.Instance.Initialize(new DbDriverConfig
            {
                DbEngineType = "mysql",
                DbServerName = "localhost",
                DbDatabaseName = "babelmetadev",
                DbDatabaseUser = "root",
                DbDatabasePassword = "",
            });

            if (MySqlDriverService.Instance.IsValidTable<Album>())
            {
                Debug.WriteLine("Album ok");
            }

            MySqlDriverService.Instance.InitializeTable<Album>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Albums);
            MySqlDriverService.Instance.InitializeTable<Artist>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Artists);
            MySqlDriverService.Instance.InitializeTable<Asset>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Assets);
            MySqlDriverService.Instance.InitializeTable<Lang>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Langs);
            MySqlDriverService.Instance.InitializeTable<Quality>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Qualities);
            MySqlDriverService.Instance.InitializeTable<Role>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Roles);
            MySqlDriverService.Instance.InitializeTable<Tag>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Tags);
            MySqlDriverService.Instance.InitializeTable<Work>();
            MySqlDriverService.Instance.InsertMany(CatalogContext.Instance.Works);
        }
    }
}
