Imports System
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Module encriptacion
    '***********************************************************************
    'pagina web post de la funcion
    'http://stackoverflow.com/questions/667887/aes-in-asp-net-with-vb-net
    '***********************************************************************
    Const HASH_SIZE As Integer = 32
    Dim msg As Array
    Public Function Encrip_Value(ByRef Val_encript As String) As String
        Try
            Dim password = "1234567890!"
            Dim salt = New Byte() {1, 2, 3, 4, 5, 6, _
             7, 8, 9, 0}
            Dim ct1 = Encrypt(password, salt, Encoding.UTF8.GetBytes(Val_encript & "| Bob; Eve;: Perform"))
            If ct1 Is Nothing Then
                Encrip_Value = "Imposible Encriptar el valor "
                Exit Function
            End If
            Val_encript = Convert.ToBase64String(ct1)
            Encrip_Value = "YES"
            Exit Function
        Catch ex As Exception
            Encrip_Value = "Error General funcion Encrip_Value " & ex.ToString
        End Try
    End Function
    Public Function Desc_Encript_Value( _
                                       ByRef Val_string As String) As String
        Try
            Dim password = "1234567890!"
            Dim salt = New Byte() {1, 2, 3, 4, 5, 6, _
             7, 8, 9, 0}
            Dim ct1() As Byte
            ct1 = Convert.FromBase64String(Val_string)
            Dim pt1 = Decrypt(password, salt, ct1)
            If pt1 Is Nothing Then
                Desc_Encript_Value = "Imposible Desencriptar el valor "
                Exit Function
            End If
            'Dim temp_Val_string As String = Convert.ToBase64String(pt1)
            Dim temp_Val_string As String = Encoding.UTF8.GetString(pt1)
            Dim Temp_val_String_matri() As String = temp_Val_string.Split("|")
            If Not Temp_val_String_matri Is Nothing Then
                Val_string = Temp_val_String_matri(0)
            End If
            Desc_Encript_Value = "YES"
        Catch ex As Exception
            Desc_Encript_Value = "Error funcion Desc_Encript_Value " & ex.Message
        End Try
    End Function
    Public Function Encrypt(ByRef password As String, _
                            ByRef passwordSalt() As Byte, _
                            ByRef plainText() As Byte) As Byte()
        Dim msg = New Byte(HASH_SIZE + (plainText.Length - 1)) {}
        Dim hash = computeHash(plainText, 0, plainText.Length)
        Buffer.BlockCopy(hash, 0, msg, 0, HASH_SIZE)
        Buffer.BlockCopy(plainText, 0, msg, HASH_SIZE, plainText.Length)
        Dim aes = createAes(password, passwordSalt)
        aes.GenerateIV()
        Dim enc = aes.CreateEncryptor()
        Dim encBytes = enc.TransformFinalBlock(msg, 0, msg.Length)
        ' Prepend IV to result 
        Dim res = New Byte(aes.IV.Length + (encBytes.Length - 1)) {}
        Buffer.BlockCopy(aes.IV, 0, res, 0, aes.IV.Length)
        Buffer.BlockCopy(encBytes, 0, res, aes.IV.Length, encBytes.Length)
        Return res

    End Function
    Public Function Decrypt(ByRef password As String, _
                            ByRef passwordSalt() As Byte, _
                            ByRef cipherText() As Byte) As Byte()
        Dim aes = createAes(password, passwordSalt)
        Dim iv = New Byte(aes.IV.Length - 1) {}
        Buffer.BlockCopy(cipherText, 0, iv, 0, iv.Length)
        aes.IV = iv
        Dim dec = aes.CreateDecryptor()
        Dim decBytes = dec.TransformFinalBlock(cipherText, iv.Length, cipherText.Length - iv.Length)
        Dim hash = computeHash(decBytes, HASH_SIZE, decBytes.Length - HASH_SIZE)
        Dim existingHash = New Byte(HASH_SIZE - 1) {}
        Buffer.BlockCopy(decBytes, 0, existingHash, 0, HASH_SIZE)
        If Not compareBytes(existingHash, hash) Then
            Throw New CryptographicException("Message hash incorrect.")
        End If
        Dim res = New Byte(decBytes.Length - HASH_SIZE - 1) {}
        Buffer.BlockCopy(decBytes, HASH_SIZE, res, 0, res.Length)
        Return res
    End Function
    Public Function createAes(ByRef password As String, _
                              ByRef salt() As Byte) As Aes
        If password.Length < 8 Then
            Throw New ArgumentException("Password must be at least 8 characters.", "password")
        End If
        If salt.Length < 8 Then
            Throw New ArgumentException("Salt must be at least 8 bytes.", "salt")
        End If
        Dim pdb = New PasswordDeriveBytes(password, salt, "SHA512", 129)
        Dim key = pdb.GetBytes(16)
        Dim aes1 = Aes.Create()
        aes1.Mode = CipherMode.CBC
        aes1.Key = pdb.GetBytes(aes1.KeySize / 8)
        Return aes1
    End Function
    Private Function compareBytes(ByVal a1 As Byte(), _
                                  ByVal a2 As Byte()) As Boolean
        If a1.Length <> a2.Length Then
            Return False
        End If
        For i As Integer = 0 To a1.Length - 1
            If a1(i) <> a2(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Function computeHash(ByRef data() As Byte, _
                                ByRef offset As Integer, _
                                ByRef count As Integer) As Byte()
        Dim sha = SHA256.Create()
        Return sha.ComputeHash(data, offset, count)
    End Function
    Public Function DesencriptaParamRue(ByVal param As String,
                                        ByRef PramDesencript As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estrctura parram RUE desencriptada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'param               : Representa la estructura enviada por el rue
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'PramDesencript  : Retorna la estructura deesencriptada 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim encryptedBytesWithSalt() As Byte = Convert.FromBase64String(param)
            Dim strSaltWord As String = "7K*9p7XQ"
            Dim salt As Byte() = New Byte(7) {}
            Dim encryptedBytes As Byte() = New Byte(encryptedBytesWithSalt.Length - salt.Length - 9) {}
            Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length)
            Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length)
            Dim iv() As Byte
            Dim key() As Byte
            DeriveKeyAndIV(strSaltWord, salt, key, iv)
            PramDesencript = DecryptStringFromBytesAes(encryptedBytes, key, iv)
            DesencriptaParamRue = "YES"
        Catch ex As Exception
            DesencriptaParamRue = "Inconsistencia general función DesencriptaParamRue " & ex.Message
        End Try
    End Function
    Function encript_md5(ByVal texto_encript As String, _
                         ByVal clave_emcript As String, _
                         ByRef texto_encriptado As String) As String
        Try
            Dim param As String = texto_encript
            Dim iv() As Byte
            Dim key() As Byte
            Dim salt(0 To 8 - 1) As Byte
            Dim rng As New RNGCryptoServiceProvider()
            rng.GetNonZeroBytes(salt)
            DeriveKeyAndIV(clave_emcript, salt, key, iv)
            ' encrypt bytes
            Dim encryptedBytes As Byte() = EncryptStringToBytesAes(param, key, iv)
            ' add salt as first 8 bytes
            Dim encryptedBytesWithSalt(0 To salt.Length + encryptedBytes.Length + 8 - 1) As Byte
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("Salted__"), 0, encryptedBytesWithSalt, 0, 8)
            Buffer.BlockCopy(salt, 0, encryptedBytesWithSalt, 8, salt.Length)
            Buffer.BlockCopy(encryptedBytes, 0, encryptedBytesWithSalt, salt.Length + 8, encryptedBytes.Length)
            ' base64 encode
            Dim paramJson As String = Convert.ToBase64String(encryptedBytesWithSalt)
            texto_encriptado = paramJson
            encript_md5 = "YES"
        Catch ex As Exception
            encript_md5 = "Inconsistencia general función encript_md5 " & ex.Message
        End Try

    End Function
    Function desc_encript_md5(ByVal texto_encript As String, _
                              ByVal clave_emcript As String, _
                              ByRef texto_desencriptado As String) As String
        Try
            Dim param As String = texto_encript
            Dim strSaltWord As String = clave_emcript
            Dim encryptedBytesWithSalt As Byte() = Convert.FromBase64String(param)
            Dim salt(0 To 8 - 1) As Byte
            Dim encryptedBytes(0 To encryptedBytesWithSalt.Length - salt.Length - 8 - 1) As Byte
            Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length)
            Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length)
            ' get key and iv
            Dim iv() As Byte
            Dim key() As Byte
            DeriveKeyAndIV(strSaltWord, salt, key, iv)
            texto_desencriptado = DecryptStringFromBytesAes(encryptedBytes, key, iv)
            desc_encript_md5 = "YES"
        Catch ex As Exception
            desc_encript_md5 = "Inconsistencia general función desc_encript_md5 " & ex.Message
        End Try
    End Function

    Function EncryptStringToBytesAes(plainText As String, _
                                     key As Byte(), _
                                     iv As Byte()) As Byte()
        ' Check arguments.
        If plainText = Nothing OrElse plainText.Length <= 0 Then
            Throw New ArgumentNullException("plainText")
        End If
        If key Is Nothing OrElse key.Length <= 0 Then
            Throw New ArgumentNullException("key")
        End If
        If iv Is Nothing OrElse iv.Length <= 0 Then
            Throw New ArgumentNullException("iv")
        End If
        ' Declare the stream used to encrypt to an in memory
        ' array of bytes.
        Dim msEncrypt As MemoryStream
        ' Declare the RijndaelManaged object
        ' used to encrypt the data.
        Dim aesAlg As RijndaelManaged = Nothing
        Try
            ' Create a RijndaelManaged object
            ' with the specified key and IV.
            ' aesAlg = New RijndaelManaged
            ' Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv
            aesAlg = New RijndaelManaged
            aesAlg.Mode = CipherMode.CBC
            aesAlg.KeySize = 256
            aesAlg.BlockSize = 128
            aesAlg.Key = key
            aesAlg.IV = iv
            ' Create an encryptor to perform the stream transform.
            Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)
            ' Create the streams used for encryption.
            msEncrypt = New MemoryStream()
            Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                Using swEncrypt As New StreamWriter(csEncrypt)
                    'Write all data to the stream.
                    swEncrypt.Write(plainText)
                    swEncrypt.Flush()
                    swEncrypt.Close()

                End Using

            End Using
            Return msEncrypt.ToArray()

        Finally
            ' Clear the RijndaelManaged object.
            If aesAlg IsNot Nothing Then
                aesAlg.Clear()
            End If

        End Try


    End Function
    Function DecryptStringFromBytesAes(ByVal cipherText() As Byte, _
                                       ByVal key() As Byte, _
                                       ByVal iv() As Byte) As String
        ' Check arguments.
        If ((cipherText Is Nothing) _
                    OrElse (cipherText.Length <= 0)) Then
            Throw New ArgumentNullException("cipherText")
        End If

        If ((key Is Nothing) _
                    OrElse (key.Length <= 0)) Then
            Throw New ArgumentNullException("key")
        End If

        If ((iv Is Nothing) _
                    OrElse (iv.Length <= 0)) Then
            Throw New ArgumentNullException("iv")
        End If

        Dim aesAlg As RijndaelManaged = Nothing
        ' Declare the string used to hold
        ' the decrypted text.

        Dim plaintext As String
        Try
            ' Create a RijndaelManaged object
            ' with the specified key and IV.
            aesAlg = New RijndaelManaged
            aesAlg.BlockSize = 128
            aesAlg.Mode = CipherMode.CBC
            aesAlg.KeySize = 256
            aesAlg.Key = key
            aesAlg.IV = iv
            'aesAlg = New RijndaelManaged() {Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, key_ = key, iv_ = iv}
            ' Create a decrytor to perform the stream transform.
            Dim decryptor As ICryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)
            ' Create the streams used for decryption.
            Dim msDecrypt As MemoryStream = New MemoryStream(cipherText)
            Dim csDecrypt As CryptoStream = New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
            Dim srDecrypt As StreamReader = New StreamReader(csDecrypt)
            ' Read the decrypted bytes from the decrypting stream
            ' and place them in a string.
            plaintext = srDecrypt.ReadToEnd
            srDecrypt.Close()
        Finally
            ' Clear the RijndaelManaged object.
            If (Not (aesAlg) Is Nothing) Then
                aesAlg.Clear()
            End If

        End Try
        Return plaintext

    End Function

    Private Sub DeriveKeyAndIV(ByVal passphrase As String, _
                               ByVal salt() As Byte, _
                               ByRef key() As Byte, _
                               ByRef iv() As Byte)
        ' generate key and iv
        Dim concatenatedHashes As List(Of Byte) = New List(Of Byte)(48)
        Dim password() As Byte = Encoding.UTF8.GetBytes(passphrase)
        Dim currentHash() As Byte = New Byte((0) - 1) {}
        Dim md5 As MD5 = md5.Create
        Dim enoughBytesForKey As Boolean = False
        ' See http://www.openssl.org/docs/crypto/EVP_BytesToKey.html#KEY_DERIVATION_ALGORITHM

        While Not enoughBytesForKey
            Dim preHashLength As Integer = (currentHash.Length _
                        + (password.Length + salt.Length))
            Dim preHash() As Byte = New Byte((preHashLength) - 1) {}
            Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length)
            Buffer.BlockCopy(password, 0, preHash, currentHash.Length, password.Length)
            Buffer.BlockCopy(salt, 0, preHash, (currentHash.Length + password.Length), salt.Length)
            currentHash = md5.ComputeHash(preHash)
            concatenatedHashes.AddRange(currentHash)
            If (concatenatedHashes.Count >= 48) Then
                enoughBytesForKey = True
            End If
        End While
        key = New Byte((32) - 1) {}
        iv = New Byte((16) - 1) {}
        concatenatedHashes.CopyTo(0, key, 0, 32)
        concatenatedHashes.CopyTo(32, iv, 0, 16)
        md5.Clear()
        md5 = Nothing
    End Sub


End Module
