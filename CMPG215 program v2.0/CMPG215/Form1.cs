using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography; //Added
using System.IO;                    //Added

namespace CMPG215
{
    public partial class Form1 : Form
    {
        string ext;
        public Form1()
        {
            InitializeComponent();
        }
        //Encrypt Button
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            saveFileDialog1.ShowDialog();
            Encrypt(openFileDialog1.FileName, saveFileDialog1.FileName, txtPass.Text);
            ext = Path.GetExtension(openFileDialog1.FileName);
        }
        //GenerateSalt
        public static byte[] GenSalt()
        {
            byte[] data = new byte[32];
            using (RNGCryptoServiceProvider rgnCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rgnCryptoServiceProvider.GetBytes(data);
            }
            return data;
        }
        //Encryption
        private void Encrypt(string inputFile, string outputFile, string password)
        {
            byte[] salt = GenSalt();
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;
            var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.CFB;
            using (FileStream fsCrypt = new FileStream(outputFile + ".aes", FileMode.Create))
            {
                fsCrypt.Write(salt, 0, salt.Length);
                using (CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        byte[] buffer = new byte[1048576];
                        int read;
                        while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, read);
                        }
                    }
                }
            }
            txtPass.Clear();
            File.Delete(inputFile); //delete unencrypted file
            MessageBox.Show("File successfully encrypted!");
        }
        //Decryption
        private void Decrypt(string inputFile, string outputFile, string password)
        {
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];
            using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
            {
                fsCrypt.Read(salt, 0, salt.Length);
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.PKCS7;
                AES.Mode = CipherMode.CFB;
                using (CryptoStream cryptoStream = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    //save to file with orginal extension
                    if (cbFileType.SelectedIndex == 0)
                    {
                        using (FileStream fsOut = new FileStream(outputFile + ".jpg", FileMode.Create))
                        {
                            try
                            {
                                int read;
                                byte[] buffer = new byte[1048576];
                                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fsOut.Write(buffer, 0, read);
                                }
                                txtPass.Clear();
                                MessageBox.Show("File successfully decrypted!");
                            }
                            catch
                            {
                                MessageBox.Show("Wrong Password!");
                                this.Close();
                            }
                        }
                    }
                    else if (cbFileType.SelectedIndex == 1)
                    {
                        using (FileStream fsOut = new FileStream(outputFile + ".txt", FileMode.Create))
                        {
                            try
                            {
                                int read;
                                byte[] buffer = new byte[1048576];
                                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fsOut.Write(buffer, 0, read);
                                }
                                txtPass.Clear();
                                MessageBox.Show("File successfully decrypted!");
                            }
                            catch
                            {
                                MessageBox.Show("Wrong Password!");
                                this.Close();
                            }
                        }
                    }
                    else if (cbFileType.SelectedIndex == 2)
                    {
                        using (FileStream fsOut = new FileStream(outputFile + ".rar", FileMode.Create))
                        {
                            try
                            {
                                int read;
                                byte[] buffer = new byte[1048576];
                                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fsOut.Write(buffer, 0, read);
                                }
                                txtPass.Clear();
                                MessageBox.Show("File successfully decrypted!");
                            }
                            catch
                            {
                                MessageBox.Show("Wrong Password!");
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        using (FileStream fsOut = new FileStream(outputFile + ext, FileMode.Create))
                        {
                            try
                            {
                                int read;
                                byte[] buffer = new byte[1048576];
                                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fsOut.Write(buffer, 0, read);
                                }
                                txtPass.Clear();
                                MessageBox.Show("File successfully decrypted!");
                            }
                            catch
                            {
                                MessageBox.Show("Wrong Password!");
                                this.Close();
                            }
                        }
                    }
                    
                }
            }
        }
        //Decrypt Button
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            saveFileDialog1.ShowDialog();
            Decrypt(openFileDialog1.FileName, saveFileDialog1.FileName, txtPass.Text);
        }
    }
}
