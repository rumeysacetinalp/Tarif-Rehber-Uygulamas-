using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TarifRehberi
{
    class VeritabaniIslemleri
    {

        string connectionString = "Server=LAPTOP-HMHG8A0R\\SQLEXPRESS;Database=Tarif;Integrated Security=True;TrustServerCertificate=True;";







    
        public void MalzemeEkle(string malzemeAdi, string toplamMiktar, string malzemeBirim, decimal birimFiyat)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "INSERT INTO Malzemeler (MalzemeAdi, ToplamMiktar, MalzemeBirim, BirimFiyat) VALUES (@MalzemeAdi, @ToplamMiktar, @MalzemeBirim, @BirimFiyat)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                    cmd.Parameters.AddWithValue("@ToplamMiktar", toplamMiktar);
                    cmd.Parameters.AddWithValue("@MalzemeBirim", malzemeBirim);
                    cmd.Parameters.AddWithValue("@BirimFiyat", birimFiyat);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Malzeme başarıyla eklendi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }




        public DataTable MalzemeAra(string malzemeAdi)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Malzemeler WHERE MalzemeAdi LIKE @MalzemeAdi";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MalzemeAdi", "%" + malzemeAdi + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            return dt;
        }

        public DataTable TumMalzemeleriGetir()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Malzemeler";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            return dt;
        }

        public void MalzemeGuncelle(int malzemeID, string malzemeAdi, string toplamMiktar, string malzemeBirim, decimal birimFiyat)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "UPDATE Malzemeler SET MalzemeAdi = @MalzemeAdi, ToplamMiktar = @ToplamMiktar, MalzemeBirim = @MalzemeBirim, BirimFiyat = @BirimFiyat WHERE MalzemeID = @MalzemeID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                    cmd.Parameters.AddWithValue("@ToplamMiktar", toplamMiktar);
                    cmd.Parameters.AddWithValue("@MalzemeBirim", malzemeBirim);
                    cmd.Parameters.AddWithValue("@BirimFiyat", birimFiyat);
                    cmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }


        public void MalzemeSil(int malzemeID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "DELETE FROM Malzemeler WHERE MalzemeID = @MalzemeID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata:  Seçtiğiniz malzeme başka bi tarife kayıtlığı olduğundan silme işlemi tamamlanamadı." );
                }
            }
        }


        public bool MalzemeVarMi(string malzemeAdi)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM Malzemeler WHERE MalzemeAdi = @MalzemeAdi";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                    return false;
                }
            }
        }

        public DataTable TarifAra(string tarifAdi)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Tarifler WHERE TarifAdi LIKE @TarifAdi";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TarifAdi", "%" + tarifAdi + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            return dt;
        }


        public DataTable TumTarifleriGetir()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Tarifler";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            return dt;
        }

        public void TarifEkle(string tarifAdi, string kategori, int hazirlamaSuresi, string talimatlar, string resimYolu, string kisiSayisi, List<(string malzemeAdi, string miktar, string birim)> malzemeler)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string duplicateCheckQuery = "SELECT COUNT(*) FROM Tarifler WHERE TarifAdi = @TarifAdi AND Kategori = @Kategori";
                    SqlCommand duplicateCheckCmd = new SqlCommand(duplicateCheckQuery, con);
                    duplicateCheckCmd.Parameters.AddWithValue("@TarifAdi", tarifAdi);
                    duplicateCheckCmd.Parameters.AddWithValue("@Kategori", kategori);

                    int count = (int)duplicateCheckCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Bu tarif zaten kaydedilmiş.");
                        return;
                    }

                    // Önce Tarifler tablosuna kaydı ekle
                    string query = "INSERT INTO Tarifler (TarifAdi, Kategori, HazirlamaSuresi, Talimatlar, ResimYolu, KisiSayisi) " +
                                   "VALUES (@TarifAdi, @Kategori, @HazirlamaSuresi, @Talimatlar, @ResimYolu, @KisiSayisi); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TarifAdi", tarifAdi);
                    cmd.Parameters.AddWithValue("@Kategori", kategori);
                    cmd.Parameters.AddWithValue("@HazirlamaSuresi", hazirlamaSuresi);
                    cmd.Parameters.AddWithValue("@Talimatlar", talimatlar);
                    cmd.Parameters.AddWithValue("@ResimYolu", resimYolu);
                    cmd.Parameters.AddWithValue("@KisiSayisi", kisiSayisi);

                 
                    int tarifID = Convert.ToInt32(cmd.ExecuteScalar());

                   
                    foreach (var malzeme in malzemeler)
                    {
                        // Malzemeler tablosunda malzemenin olup olmadığını kontrol et
                        string kontrolQuery = "SELECT MalzemeID FROM Malzemeler WHERE MalzemeAdi = @MalzemeAdi";
                        SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, con);
                        kontrolCmd.Parameters.AddWithValue("@MalzemeAdi", malzeme.malzemeAdi);
                        object malzemeIDObj = kontrolCmd.ExecuteScalar();

                        int malzemeID;
                        if (malzemeIDObj == null)
                        {
                            // Malzeme yoksa, ekle 
                            string malzemeEkleQuery = "INSERT INTO Malzemeler (MalzemeAdi, ToplamMiktar, MalzemeBirim,BirimFiyat) VALUES (@MalzemeAdi, 0, @MalzemeBirim,@BirimFiyat); SELECT SCOPE_IDENTITY();";
                            SqlCommand malzemeEkleCmd = new SqlCommand(malzemeEkleQuery, con);
                            malzemeEkleCmd.Parameters.AddWithValue("@MalzemeAdi", malzeme.malzemeAdi);
                            malzemeEkleCmd.Parameters.AddWithValue("@MalzemeBirim", malzeme.birim);
                            malzemeEkleCmd.Parameters.AddWithValue("@BirimFiyat", 0); 
                            malzemeID = Convert.ToInt32(malzemeEkleCmd.ExecuteScalar());
                        }
                        else
                        {
                            // Malzeme mevcutsa malzemeID'yi kullan
                            malzemeID = Convert.ToInt32(malzemeIDObj);
                        }

                        
                        TarifMalzemeEkle(tarifID, malzemeID, malzeme.miktar, malzeme.birim);
                    }

                    MessageBox.Show("Tarif ve malzemeler başarıyla eklendi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }


        public void TarifMalzemeEkle(int tarifID, int malzemeID, string miktar, string birim)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                   
                    string kontrolQuery = "SELECT COUNT(*) FROM TarifMalzeme WHERE TarifID = @TarifID AND MalzemeID = @MalzemeID";
                    SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, con);
                    kontrolCmd.Parameters.AddWithValue("@TarifID", tarifID);
                    kontrolCmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                    int count = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                    
                    if (count > 0)
                    {
                        MessageBox.Show("Bu malzeme bu tarif için zaten eklenmiş.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    
                    string query = "INSERT INTO TarifMalzeme (TarifID, MalzemeID, MalzemeMiktar, malzemeBirim) VALUES (@TarifID, @MalzemeID, @MalzemeMiktar, @malzemeBirim)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TarifID", tarifID);
                    cmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                    cmd.Parameters.AddWithValue("@MalzemeMiktar", miktar);
                    cmd.Parameters.AddWithValue("@malzemeBirim", birim);

                   
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Tarif malzemesi başarıyla eklendi.");
                    }
                    else
                    {
                        MessageBox.Show("Tarif malzemesi eklenemedi.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }



        public void TarifGuncelle(int tarifID, string tarifAdi, string kategori, int hazirlamaSuresi, string talimatlar, string resimYolu, string kisiSayisi, List<(string malzemeAdi, string miktar, string birim)> malzemeler)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                   
                    string duplicateCheckQuery = "SELECT COUNT(*) FROM Tarifler WHERE TarifAdi = @TarifAdi AND Kategori = @Kategori AND TarifID != @TarifID";
                    SqlCommand duplicateCheckCmd = new SqlCommand(duplicateCheckQuery, con);
                    duplicateCheckCmd.Parameters.AddWithValue("@TarifAdi", tarifAdi);
                    duplicateCheckCmd.Parameters.AddWithValue("@Kategori", kategori);
                    duplicateCheckCmd.Parameters.AddWithValue("@TarifID", tarifID);

                    int duplicateCount = (int)duplicateCheckCmd.ExecuteScalar();
                    if (duplicateCount > 0)
                    {
                        MessageBox.Show("Bu tarif adı ve kategori zaten mevcut.");
                        return;
                    }


                    
                    string query = "UPDATE Tarifler SET TarifAdi = @TarifAdi, Kategori = @Kategori, HazirlamaSuresi = @HazirlamaSuresi, Talimatlar = @Talimatlar, ResimYolu = @ResimYolu, KisiSayisi = @KisiSayisi WHERE TarifID = @TarifID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TarifID", tarifID);
                    cmd.Parameters.AddWithValue("@TarifAdi", tarifAdi);
                    cmd.Parameters.AddWithValue("@Kategori", kategori);
                    cmd.Parameters.AddWithValue("@HazirlamaSuresi", hazirlamaSuresi);
                    cmd.Parameters.AddWithValue("@Talimatlar", talimatlar);
                    cmd.Parameters.AddWithValue("@ResimYolu", resimYolu);
                    cmd.Parameters.AddWithValue("@KisiSayisi", kisiSayisi);
                    cmd.ExecuteNonQuery();


                   
                    string deleteQuery = "DELETE FROM TarifMalzeme WHERE TarifID = @TarifID";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, con);
                    deleteCmd.Parameters.AddWithValue("@TarifID", tarifID);
                    deleteCmd.ExecuteNonQuery();

                    
                    foreach (var malzeme in malzemeler)
                    {
                        
                        string kontrolQuery = "SELECT MalzemeID FROM Malzemeler WHERE MalzemeAdi = @MalzemeAdi";
                        SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, con);
                        kontrolCmd.Parameters.AddWithValue("@MalzemeAdi", malzeme.malzemeAdi);
                        object malzemeIDObj = kontrolCmd.ExecuteScalar();

                        int malzemeID;
                        if (malzemeIDObj == null)
                        {
                            
                            string malzemeEkleQuery = "INSERT INTO Malzemeler (MalzemeAdi, ToplamMiktar, MalzemeBirim, BirimFiyat) VALUES (@MalzemeAdi, 0, @MalzemeBirim, 0); SELECT SCOPE_IDENTITY();";
                            SqlCommand malzemeEkleCmd = new SqlCommand(malzemeEkleQuery, con);
                            malzemeEkleCmd.Parameters.AddWithValue("@MalzemeAdi", malzeme.malzemeAdi);
                            malzemeEkleCmd.Parameters.AddWithValue("@MalzemeBirim", malzeme.birim);
                            malzemeID = Convert.ToInt32(malzemeEkleCmd.ExecuteScalar());
                        }
                        else
                        {
                            
                            malzemeID = Convert.ToInt32(malzemeIDObj);
                        }

                       
                        string tarifMalzemeKontrolQuery = "SELECT COUNT(*) FROM TarifMalzeme WHERE TarifID = @TarifID AND MalzemeID = @MalzemeID";
                        SqlCommand tarifMalzemeKontrolCmd = new SqlCommand(tarifMalzemeKontrolQuery, con);
                        tarifMalzemeKontrolCmd.Parameters.AddWithValue("@TarifID", tarifID);
                        tarifMalzemeKontrolCmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                        int count = Convert.ToInt32(tarifMalzemeKontrolCmd.ExecuteScalar());

                        if (count > 0)
                        {
                           
                            string tarifMalzemeGuncelleQuery = "UPDATE TarifMalzeme SET MalzemeMiktar = @MalzemeMiktar, malzemeBirim = @malzemeBirim WHERE TarifID = @TarifID AND MalzemeID = @MalzemeID";
                            SqlCommand tarifMalzemeGuncelleCmd = new SqlCommand(tarifMalzemeGuncelleQuery, con);
                            tarifMalzemeGuncelleCmd.Parameters.AddWithValue("@TarifID", tarifID);
                            tarifMalzemeGuncelleCmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                            tarifMalzemeGuncelleCmd.Parameters.AddWithValue("@MalzemeMiktar", malzeme.miktar);
                            tarifMalzemeGuncelleCmd.Parameters.AddWithValue("@malzemeBirim", malzeme.birim);
                            tarifMalzemeGuncelleCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            
                            string tarifMalzemeEkleQuery = "INSERT INTO TarifMalzeme (TarifID, MalzemeID, MalzemeMiktar, malzemeBirim) VALUES (@TarifID, @MalzemeID, @MalzemeMiktar, @malzemeBirim)";
                            SqlCommand tarifMalzemeEkleCmd = new SqlCommand(tarifMalzemeEkleQuery, con);
                            tarifMalzemeEkleCmd.Parameters.AddWithValue("@TarifID", tarifID);
                            tarifMalzemeEkleCmd.Parameters.AddWithValue("@MalzemeID", malzemeID);
                            tarifMalzemeEkleCmd.Parameters.AddWithValue("@MalzemeMiktar", malzeme.miktar);
                            tarifMalzemeEkleCmd.Parameters.AddWithValue("@malzemeBirim", malzeme.birim);
                            tarifMalzemeEkleCmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Tarif ve malzemeler başarıyla güncellendi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }





        public void TarifSil(int tarifID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                   
                    string silTarifMalzemeQuery = "DELETE FROM TarifMalzeme WHERE TarifID = @TarifID";
                    SqlCommand silTarifMalzemeCmd = new SqlCommand(silTarifMalzemeQuery, con);
                    silTarifMalzemeCmd.Parameters.AddWithValue("@TarifID", tarifID);
                    silTarifMalzemeCmd.ExecuteNonQuery();

              
                    string silTarifQuery = "DELETE FROM Tarifler WHERE TarifID = @TarifID";
                    SqlCommand silTarifCmd = new SqlCommand(silTarifQuery, con);
                    silTarifCmd.Parameters.AddWithValue("@TarifID", tarifID);
                    silTarifCmd.ExecuteNonQuery();

                    MessageBox.Show("Tarif ve ilişkili malzemeler başarıyla silindi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        public List<(string malzemeAdi, string miktar, string birim)> TarifMalzemeleriGetir(int tarifID)
        {
            List<(string malzemeAdi, string miktar, string birim)> malzemeler = new List<(string, string, string)>();

            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                tm.TarifID,
                m.MalzemeAdi,
                tm.MalzemeMiktar,
                tm.malzemeBirim
            FROM 
                TarifMalzeme tm
            JOIN 
                Malzemeler m ON tm.MalzemeID = m.MalzemeID
            WHERE 
                tm.TarifID = @TarifID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TarifID", tarifID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string malzemeAdi = reader["MalzemeAdi"].ToString();
                        string miktar = reader["MalzemeMiktar"].ToString();
                        string birim = reader["malzemeBirim"].ToString();

                        malzemeler.Add((malzemeAdi, miktar, birim));
                    }
                }
            }

            return malzemeler;
        }


       
        public DataTable TarifArama(string tarifAdi, string kategori)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Tarifler WHERE 1=1";

                
                if (!string.IsNullOrWhiteSpace(tarifAdi))
                {
                    query += " AND TarifAdi LIKE @tarifAdi";
                }
                if (!string.IsNullOrWhiteSpace(kategori))
                {
                    query += " AND Kategori = @kategori";
                }

                SqlCommand cmd = new SqlCommand(query, conn);

               
                if (!string.IsNullOrWhiteSpace(tarifAdi))
                {
                    cmd.Parameters.AddWithValue("@tarifAdi", "%" + tarifAdi + "%");
                }
                if (!string.IsNullOrWhiteSpace(kategori))
                {
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                }

                
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetTarifDetaylari(int tarifID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT t.TarifAdi, t.Kategori, t.HazirlamaSuresi, t.Talimatlar, t.ResimYolu, t.KisiSayisi,
                   m.MalzemeAdi, tm.MalzemeMiktar, tm.malzemeBirim
            FROM Tarifler t
            INNER JOIN TarifMalzeme tm ON t.TarifID = tm.TarifID
            INNER JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID
            WHERE t.TarifID = @tarifID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@tarifID", tarifID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Sorgudan hiçbir veri dönmedi. TarifID: " + tarifID);
            }

            return dt;
        }


        public DataTable TarifleriMalzemeyeGoreGetir(List<string> seciliMalzemeler)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
               
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("SELECT t.TarifID, t.TarifAdi, t.Kategori, t.HazirlamaSuresi, t.KisiSayisi, t.ResimYolu, ");
                queryBuilder.Append("COUNT(tm.MalzemeID) AS EslesmeSayisi ");
                queryBuilder.Append("FROM Tarifler t ");
                queryBuilder.Append("JOIN TarifMalzeme tm ON t.TarifID = tm.TarifID ");
                queryBuilder.Append("JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID ");
                queryBuilder.Append("WHERE m.MalzemeAdi IN (");

               
                for (int i = 0; i < seciliMalzemeler.Count; i++)
                {
                    queryBuilder.Append($"@malzeme{i}");
                    if (i < seciliMalzemeler.Count - 1)
                        queryBuilder.Append(", ");
                }

                queryBuilder.Append(") ");
                queryBuilder.Append("GROUP BY t.TarifID, t.TarifAdi, t.Kategori, t.HazirlamaSuresi, t.KisiSayisi, t.ResimYolu ");
                queryBuilder.Append("ORDER BY COUNT(tm.MalzemeID) DESC");

                SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn);

              
                for (int i = 0; i < seciliMalzemeler.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@malzeme{i}", seciliMalzemeler[i]);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }



        public DataTable FiltreliTarifleriGetir(int? minMalzemeSayisi, int? maxMalzemeSayisi, string kategori, int? minKisiSayisi, int? maxKisiSayisi, decimal? minMaliyet, decimal? maxMaliyet)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("SELECT t.TarifID, t.TarifAdi, t.Kategori, t.HazirlamaSuresi, t.ResimYolu, t.KisiSayisi, ");
                queryBuilder.Append("SUM(tm.MalzemeMiktar * m.BirimFiyat) AS ToplamMaliyet, COUNT(tm.MalzemeID) AS MalzemeSayisi ");
                queryBuilder.Append("FROM Tarifler t ");
                queryBuilder.Append("JOIN TarifMalzeme tm ON t.TarifID = tm.TarifID ");
                queryBuilder.Append("JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID ");

               
                bool hasWhereClause = false;

                if (!string.IsNullOrEmpty(kategori) || minKisiSayisi.HasValue || maxKisiSayisi.HasValue || minMaliyet.HasValue || maxMaliyet.HasValue)
                {
                    queryBuilder.Append("WHERE ");
                    hasWhereClause = true;

                    if (!string.IsNullOrEmpty(kategori))
                    {
                        queryBuilder.Append("t.Kategori = @kategori ");
                    }

                    if (minKisiSayisi.HasValue)
                    {
                        if (!string.IsNullOrEmpty(kategori))
                            queryBuilder.Append("AND ");
                        queryBuilder.Append("t.KisiSayisi >= @minKisiSayisi ");
                    }

                    if (maxKisiSayisi.HasValue)
                    {
                        if (!string.IsNullOrEmpty(kategori) || minKisiSayisi.HasValue)
                            queryBuilder.Append("AND ");
                        queryBuilder.Append("t.KisiSayisi <= @maxKisiSayisi ");
                    }
                }

               
                queryBuilder.Append("GROUP BY t.TarifID, t.TarifAdi, t.Kategori, t.HazirlamaSuresi, t.ResimYolu, t.KisiSayisi ");

               
                List<string> havingConditions = new List<string>();

                if (minMalzemeSayisi.HasValue)
                {
                    havingConditions.Add("COUNT(tm.MalzemeID) >= @minMalzemeSayisi");
                }
                if (maxMalzemeSayisi.HasValue)
                {
                    havingConditions.Add("COUNT(tm.MalzemeID) <= @maxMalzemeSayisi");
                }

                if (minMaliyet.HasValue)
                {
                    havingConditions.Add("SUM(tm.MalzemeMiktar * m.BirimFiyat) >= @minMaliyet");
                }

                if (maxMaliyet.HasValue)
                {
                    havingConditions.Add("SUM(tm.MalzemeMiktar * m.BirimFiyat) <= @maxMaliyet");
                }

                if (havingConditions.Count > 0)
                {
                    queryBuilder.Append("HAVING ");
                    queryBuilder.Append(string.Join(" AND ", havingConditions));
                }

                SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), con);

                
                if (minMalzemeSayisi.HasValue)
                {
                    cmd.Parameters.AddWithValue("@minMalzemeSayisi", minMalzemeSayisi.Value);
                }
                if (maxMalzemeSayisi.HasValue)
                {
                    cmd.Parameters.AddWithValue("@maxMalzemeSayisi", maxMalzemeSayisi.Value);
                }

                if (!string.IsNullOrEmpty(kategori))
                {
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                }

                if (minKisiSayisi.HasValue)
                {
                    cmd.Parameters.AddWithValue("@minKisiSayisi", minKisiSayisi.Value);
                }

                if (maxKisiSayisi.HasValue)
                {
                    cmd.Parameters.AddWithValue("@maxKisiSayisi", maxKisiSayisi.Value);
                }

                if (minMaliyet.HasValue)
                {
                    cmd.Parameters.AddWithValue("@minMaliyet", minMaliyet.Value);
                }

                if (maxMaliyet.HasValue)
                {
                    cmd.Parameters.AddWithValue("@maxMaliyet", maxMaliyet.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }


        public decimal GetTarifMaliyeti(int tarifID)
        {
            decimal toplamMaliyet = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT SUM(tm.MalzemeMiktar * m.BirimFiyat) AS ToplamMaliyet
                         FROM TarifMalzeme tm
                         JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID
                         WHERE tm.TarifID = @TarifID
                         GROUP BY tm.TarifID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TarifID", tarifID);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    toplamMaliyet = Convert.ToDecimal(result);
                }
            }
            return toplamMaliyet;
        }



        public List<string> EksikMalzemeleriKontrolEt(int tarifID)
        {
            List<string> eksikMalzemeler = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT m.MalzemeAdi, tm.MalzemeMiktar, m.ToplamMiktar
        FROM TarifMalzeme tm
        JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID
        WHERE tm.TarifID = @tarifID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tarifID", tarifID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double gerekliMiktar = Convert.ToDouble(reader["MalzemeMiktar"]);
                        double mevcutStok = Convert.ToDouble(reader["ToplamMiktar"]);

                        if (gerekliMiktar > mevcutStok)
                        {
                            eksikMalzemeler.Add(reader["MalzemeAdi"].ToString());
                        }
                    }
                }
            }

            return eksikMalzemeler;
        }

        public decimal EksikMalzemelerinMaliyetiniHesapla(int tarifID)
        {
            decimal eksikMalzemelerMaliyeti = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT m.MalzemeAdi, tm.MalzemeMiktar, m.ToplamMiktar, m.BirimFiyat
                FROM TarifMalzeme tm
                JOIN Malzemeler m ON tm.MalzemeID = m.MalzemeID
                WHERE tm.TarifID = @tarifID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tarifID", tarifID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double gerekliMiktar = Convert.ToDouble(reader["MalzemeMiktar"]);
                        double mevcutStok = Convert.ToDouble(reader["ToplamMiktar"]);
                        decimal birimFiyat = Convert.ToDecimal(reader["BirimFiyat"]);

                        if (gerekliMiktar > mevcutStok)
                        {
                            double eksikMiktar = gerekliMiktar - mevcutStok;
                            eksikMalzemelerMaliyeti += (decimal)eksikMiktar * birimFiyat;
                        }
                    }
                }
            }

            return eksikMalzemelerMaliyeti;
        }


        public int TarifMalzemeSayisiGetir(int tarifID)
        {
            int malzemeSayisi = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM TarifMalzeme WHERE TarifID = @TarifID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TarifID", tarifID);
                    malzemeSayisi = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            return malzemeSayisi;
        }



    }
}
