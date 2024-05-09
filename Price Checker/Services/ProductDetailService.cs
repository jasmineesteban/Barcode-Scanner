using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Price_Checker.Services
{
    internal class ProductDetailService
    {
        private readonly string connstring;
        private DatabaseConfig _config;


        private Form formInstance;

        public ProductDetailService(Form form)
        {
            _config = new DatabaseConfig();
            connstring = $"server={_config.Server};port={_config.Port};uid={_config.Uid};pwd={_config.Pwd};database={_config.Database}";
            formInstance = form;
        }


        public void HandleProductDetails(string barcode, Label lbl_name, Label lbl_price, Label lbl_manufacturer, Label lbl_uom, Label lbl_generic)
        {
            List<Product> products = GetProductDetails(barcode);

            if (products.Count == 1)
            {
                lbl_name.Text = products[0].Name;
                lbl_price.Text = products[0].Price;
                lbl_manufacturer.Text = products[0].Manufacturer;
                lbl_uom.Text = products[0].UOM;
                lbl_generic.Text = products[0].Generic;
            }
            else if (products.Count > 1)
            {
                // Prompt user to choose the product
                using (pop chooseProductForm = new pop(products))
                {
                    DialogResult result = chooseProductForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        lbl_name.Text = chooseProductForm.SelectedProduct.Name;
                        lbl_price.Text = chooseProductForm.SelectedProduct.Price;
                        lbl_manufacturer.Text = chooseProductForm.SelectedProduct.Manufacturer;
                        lbl_uom.Text = chooseProductForm.SelectedProduct.UOM;
                        lbl_generic.Text = chooseProductForm.SelectedProduct.Generic;
                    }
                    else
                    {
                        lbl_name.Text = "N/A";
                        lbl_price.Text = "N/A";
                        lbl_manufacturer.Text = "N/A";
                        lbl_uom.Text = "N/A";
                        lbl_generic.Text = "N/A";

                    }
                }
            }
            else
            {
                MessageBox.Show("Product Not Found");
            }

            // Timer logic
            Timer timer = new Timer();
            timer.Interval = 3000; // Form interval to 3 seconds
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
           formInstance.Close();
        }

        public List<Product> GetProductDetails(string barcode)
        {
            List<Product> products = new List<Product>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();
                    string sql = "SELECT * FROM prod_verifier WHERE prod_barcode = @Barcode";
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Barcode", barcode);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Product product = new Product();
                        product.Name = reader["prod_description"].ToString();
                        product.Price = "₱ " + reader["prod_price"].ToString();
                        product.Manufacturer = reader["prod_pincipal"].ToString();
                        product.UOM = "per " + reader["prod_uom"].ToString();
                        product.Generic = reader["prod_generic"].ToString();
                        products.Add(product);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }

            return products;
        }
    }
}