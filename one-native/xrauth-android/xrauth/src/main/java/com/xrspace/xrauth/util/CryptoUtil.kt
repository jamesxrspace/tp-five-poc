package com.xrspace.xrauth.util

import android.os.Build
import android.security.keystore.KeyGenParameterSpec
import android.security.keystore.KeyProperties
import android.text.TextUtils
import android.util.Base64
import android.util.Log
import androidx.annotation.VisibleForTesting
import java.io.IOException
import java.math.BigInteger
import java.security.InvalidAlgorithmParameterException
import java.security.InvalidKeyException
import java.security.KeyPairGenerator
import java.security.KeyStore
import java.security.KeyStoreException
import java.security.NoSuchAlgorithmException
import java.security.NoSuchProviderException
import java.security.PrivateKey
import java.security.ProviderException
import java.security.UnrecoverableEntryException
import java.security.cert.CertificateException
import java.security.spec.AlgorithmParameterSpec
import java.util.Calendar
import javax.crypto.BadPaddingException
import javax.crypto.Cipher
import javax.crypto.IllegalBlockSizeException
import javax.crypto.KeyGenerator
import javax.crypto.NoSuchPaddingException
import javax.crypto.SecretKey
import javax.crypto.spec.IvParameterSpec
import javax.crypto.spec.SecretKeySpec
import javax.security.auth.x500.X500Principal

/**
 * Created by lbalmaceda on 8/24/17.
 * Class to handle encryption/decryption cryptographic operations using AES and RSA algorithms in devices with API 19 or higher.
 * @author Dewei.Chen@xrspace.io
 */
@SuppressWarnings("WeakerAccess")
class CryptoUtil(private val storage: IXrStorage, keyAlias: String) {
    companion object {
        private const val TAG = "[XrAuth]CryptoUtil"

        // Transformations available since API 18
        // https://developer.android.com/training/articles/keystore.html#SupportedCiphers
        private const val RSA_TRANSFORMATION = "RSA/ECB/PKCS1Padding"

        // https://developer.android.com/reference/javax/crypto/Cipher.html
        private const val AES_TRANSFORMATION = "AES/GCM/NOPADDING"

        private const val ANDROID_KEY_STORE = "AndroidKeyStore"
        private const val ALGORITHM_RSA = "RSA"
        private const val ALGORITHM_AES = "AES"
        private const val AES_KEY_SIZE = 256
        private const val RSA_KEY_SIZE = 2048
    }

    private var KEY_ALIAS: String
    private var KEY_IV_ALIAS: String

    init {
        var alias = keyAlias
        alias = alias.trim { it <= ' ' }
        require(!TextUtils.isEmpty(alias)) { "RSA and AES Key alias must be valid." }
        this.KEY_ALIAS = alias
        KEY_IV_ALIAS = alias + "_iv"
    }


    @VisibleForTesting
    @Throws(CryptoException::class, IncompatibleDeviceException::class)
    fun getRSAKeyEntry(): KeyStore.PrivateKeyEntry? {
        return try {
            val keyStore = KeyStore.getInstance(ANDROID_KEY_STORE)
            keyStore.load(null)
            if (keyStore.containsAlias(KEY_ALIAS)) {
                //Return existing key. On weird cases, the alias would be present but the key not
                val existingKey = getKeyEntryCompat(keyStore)
                if (existingKey != null) {
                    return existingKey
                }
            }
            val start = Calendar.getInstance()
            val end = Calendar.getInstance()
            end.add(Calendar.YEAR, 25)
            val spec: AlgorithmParameterSpec
            val principal = X500Principal("CN=XRSPACE.Android,O=XRSPACE")
            spec = KeyGenParameterSpec.Builder(
                KEY_ALIAS,
                KeyProperties.PURPOSE_DECRYPT or KeyProperties.PURPOSE_ENCRYPT
            )
                .setCertificateSubject(principal)
                .setCertificateSerialNumber(BigInteger.ONE)
                .setCertificateNotBefore(start.time)
                .setCertificateNotAfter(end.time)
                .setKeySize(RSA_KEY_SIZE)
                .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_RSA_PKCS1)
                .setBlockModes(KeyProperties.BLOCK_MODE_ECB)
                .build()
            val generator: KeyPairGenerator =
                KeyPairGenerator.getInstance(ALGORITHM_RSA, ANDROID_KEY_STORE)
            generator.initialize(spec)
            generator.generateKeyPair()
            getKeyEntryCompat(keyStore)
        } catch (e: CertificateException) {
            /*
                 * This exceptions are safe to be ignored:
                 *
                 * - CertificateException:
                 *      Thrown when certificate has expired (25 years..) or couldn't be loaded
                 * - KeyStoreException:
                 * - NoSuchProviderException:
                 *      Thrown when "AndroidKeyStore" is not available. Was introduced on API 18.
                 * - NoSuchAlgorithmException:
                 *      Thrown when "RSA" algorithm is not available. Was introduced on API 18.
                 * - InvalidAlgorithmParameterException:
                 *      Thrown if Key Size is other than 512, 768, 1024, 2048, 3072, 4096
                 *      or if Padding is other than RSA/ECB/PKCS1Padding, introduced on API 18
                 *      or if Block Mode is other than ECB
                 * - ProviderException:
                 *      Thrown on some modified devices when KeyPairGenerator#generateKeyPair is called.
                 *      See: https://www.bountysource.com/issues/45527093-keystore-issues
                 *
                 * However if any of this exceptions happens to be thrown (OEMs often change their Android distribution source code),
                 * all the checks performed in this class wouldn't matter and the device would not be compatible at all with it.
                 *
                 * Read more in https://developer.android.com/training/articles/keystore#SupportedAlgorithms
                 */
            Log.e(TAG, "The device can't generate a new RSA Key pair.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: InvalidAlgorithmParameterException) {
            Log.e(TAG, "The device can't generate a new RSA Key pair.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: NoSuchProviderException) {
            Log.e(TAG, "The device can't generate a new RSA Key pair.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: NoSuchAlgorithmException) {
            Log.e(TAG, "The device can't generate a new RSA Key pair.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: KeyStoreException) {
            Log.e(TAG, "The device can't generate a new RSA Key pair.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: ProviderException) {
            Log.e(TAG, "The device can't generate a new RSA Key pair.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: IOException) {
            /*
                 * Any of this exceptions mean the old key pair is somehow corrupted.
                 * We can delete both the RSA and the AES keys and let the user retry the operation.
                 *
                 * - IOException:
                 *      Thrown when there is an I/O or format problem with the keystore data.
                 * - UnrecoverableEntryException:
                 *      Thrown when the key cannot be recovered. Probably because it was invalidated by a Lock Screen change.
                 */
            deleteRSAKeys()
            deleteAESKeys()
            throw CryptoException(
                "The existing RSA key pair could not be recovered and has been deleted. " +
                        "This occasionally happens when the Lock Screen settings are changed. You can safely retry this operation.",
                e
            )
        } catch (e: UnrecoverableEntryException) {
            deleteRSAKeys()
            deleteAESKeys()
            throw CryptoException(
                "The existing RSA key pair could not be recovered and has been deleted. " +
                        "This occasionally happens when the Lock Screen settings are changed. You can safely retry this operation.",
                e
            )
        }
    }

    /**
     * Helper method compatible with older Android versions to load the Private Key Entry from
     * the KeyStore using the [.KEY_ALIAS].
     *
     * @param keyStore the KeyStore instance. Must be initialized (loaded).
     * @return the key entry stored in the KeyStore or null if not present.
     * @throws KeyStoreException           if the keystore was not initialized.
     * @throws NoSuchAlgorithmException    if device is not compatible with RSA algorithm. RSA is available since API 18.
     * @throws UnrecoverableEntryException if key cannot be recovered. Probably because it was invalidated by a Lock Screen change.
     */
    @Throws(
        KeyStoreException::class,
        NoSuchAlgorithmException::class,
        UnrecoverableEntryException::class
    )
    private fun getKeyEntryCompat(keyStore: KeyStore): KeyStore.PrivateKeyEntry? {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
            return keyStore.getEntry(KEY_ALIAS, null) as KeyStore.PrivateKeyEntry
        }

        //Following code is for API 28+
        val privateKey = keyStore.getKey(KEY_ALIAS, null) as PrivateKey
        val certificate = keyStore.getCertificate(KEY_ALIAS) ?: return null
        return KeyStore.PrivateKeyEntry(privateKey, arrayOf(certificate))
    }

    /**
     * Removes the RSA keys generated in a previous execution.
     * Used when we want the next call to [.encrypt] or [.decrypt]
     * to recreate the keys.
     */
    private fun deleteRSAKeys() {
        try {
            val keyStore = KeyStore.getInstance(ANDROID_KEY_STORE)
            keyStore.load(null)
            keyStore.deleteEntry(KEY_ALIAS)
            Log.d(TAG, "Deleting the existing RSA key pair from the KeyStore.")
        } catch (e: KeyStoreException) {
            Log.e(TAG, "Failed to remove the RSA KeyEntry from the Android KeyStore.", e)
        } catch (e: CertificateException) {
            Log.e(TAG, "Failed to remove the RSA KeyEntry from the Android KeyStore.", e)
        } catch (e: IOException) {
            Log.e(TAG, "Failed to remove the RSA KeyEntry from the Android KeyStore.", e)
        } catch (e: NoSuchAlgorithmException) {
            Log.e(TAG, "Failed to remove the RSA KeyEntry from the Android KeyStore.", e)
        }
    }

    /**
     * Removes the AES keys generated in a previous execution.
     * Used when we want the next call to [.encrypt] or [.decrypt]
     * to recreate the keys.
     */
    private fun deleteAESKeys() {
        storage.remove(KEY_ALIAS)
        storage.remove(KEY_IV_ALIAS)
    }

    /**
     * Decrypts the given input using a generated RSA Private Key.
     * Used to decrypt the AES key for later usage.
     *
     * @param encryptedInput the input bytes to decrypt
     * @return the decrypted bytes output
     * @throws IncompatibleDeviceException in the event the device can't understand the cryptographic settings required
     * @throws CryptoException             if the stored RSA keys can't be recovered and should be deemed invalid
     */
    @VisibleForTesting
    @Throws(IncompatibleDeviceException::class, CryptoException::class)
    fun rsaDecrypt(encryptedInput: ByteArray): ByteArray? {
        return try {
            val privateKey = getRSAKeyEntry()!!.privateKey
            val cipher = Cipher.getInstance(RSA_TRANSFORMATION)
            cipher.init(Cipher.DECRYPT_MODE, privateKey)
            cipher.doFinal(encryptedInput)
        } catch (e: NoSuchAlgorithmException) {
            /*
                  * This exceptions are safe to be ignored:
                  *
                  * - NoSuchPaddingException:
                  *      Thrown if PKCS1Padding is not available. Was introduced in API 1.
                  * - NoSuchAlgorithmException:
                  *      Thrown if the transformation is null, empty or invalid, or if no security provider
                  *      implements it. Was introduced in API 1.
                  * - InvalidKeyException:
                  *      Thrown if the given key is inappropriate for initializing this cipher.
                  *
                  * Read more in https://developer.android.com/reference/javax/crypto/Cipher
                  */
            Log.e(TAG, "The device can't decrypt input using a RSA Key.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: NoSuchPaddingException) {
            Log.e(TAG, "The device can't decrypt input using a RSA Key.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: InvalidKeyException) {
            Log.e(TAG, "The device can't decrypt input using a RSA Key.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: IllegalArgumentException) {
            /*
                  * Any of this exceptions mean the encrypted input is somehow corrupted and cannot be recovered.
                  * Delete the AES keys since those originated the input.
                  *
                  * - IllegalBlockSizeException:
                  *      Thrown only on encrypt mode.
                  * - BadPaddingException:
                  *      Thrown if the input doesn't contain the proper padding bytes.
                  * - IllegalArgumentException
                  *      Thrown when doFinal is called with a null input.
                  */
            deleteAESKeys()
            throw CryptoException(
                "The RSA encrypted input is corrupted and cannot be recovered. Please discard it.",
                e
            )
        } catch (e: IllegalBlockSizeException) {
            deleteAESKeys()
            throw CryptoException(
                "The RSA encrypted input is corrupted and cannot be recovered. Please discard it.",
                e
            )
        } catch (e: BadPaddingException) {
            deleteAESKeys()
            throw CryptoException(
                "The RSA encrypted input is corrupted and cannot be recovered. Please discard it.",
                e
            )
        }
    }

    /**
     * Encrypts the given input using a generated RSA Public Key.
     * Used to encrypt the AES key for later storage.
     *
     * @param decryptedInput the input bytes to encrypt
     * @return the encrypted bytes output
     * @throws IncompatibleDeviceException in the event the device can't understand the cryptographic settings required
     * @throws CryptoException             if the stored RSA keys can't be recovered and should be deemed invalid
     */
    @VisibleForTesting
    @Throws(IncompatibleDeviceException::class, CryptoException::class)
    fun rsaEncrypt(decryptedInput: ByteArray?): ByteArray {
        return try {
            val certificate = getRSAKeyEntry()!!.certificate
            val cipher = Cipher.getInstance(RSA_TRANSFORMATION)
            cipher.init(Cipher.ENCRYPT_MODE, certificate)
            cipher.doFinal(decryptedInput)
        } catch (e: NoSuchAlgorithmException) {
            /*
                  * This exceptions are safe to be ignored:
                  *
                  * - NoSuchPaddingException:
                  *      Thrown if PKCS1Padding is not available. Was introduced in API 1.
                  * - NoSuchAlgorithmException:
                  *      Thrown if the transformation is null, empty or invalid, or if no security provider
                  *      implements it. Was introduced in API 1.
                  * - InvalidKeyException:
                  *      Thrown if the given key is inappropriate for initializing this cipher.
                  *
                  * Read more in https://developer.android.com/reference/javax/crypto/Cipher
                  */
            Log.e(TAG, "The device can't encrypt input using a RSA Key.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: NoSuchPaddingException) {
            Log.e(TAG, "The device can't encrypt input using a RSA Key.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: InvalidKeyException) {
            Log.e(TAG, "The device can't encrypt input using a RSA Key.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: IllegalBlockSizeException) {
            /*
                  * They really should not be thrown at all since padding is requested in the transformation.
                  * Delete the AES keys since those originated the input.
                  *
                  * - IllegalBlockSizeException:
                  *      Thrown if no padding has been requested and the length is not multiple of block size.
                  * - BadPaddingException:
                  *      Thrown only on decrypt mode.
                  */
            deleteAESKeys()
            throw CryptoException("The RSA decrypted input is invalid.", e)
        } catch (e: BadPaddingException) {
            deleteAESKeys()
            throw CryptoException("The RSA decrypted input is invalid.", e)
        }
    }

    /**
     * Attempts to recover the existing AES Key or generates a new one if none is found.
     *
     * @return a valid  AES Key bytes
     * @throws IncompatibleDeviceException in the event the device can't understand the cryptographic settings required
     * @throws CryptoException             if the stored RSA keys can't be recovered and should be deemed invalid
     */
    @VisibleForTesting
    @Throws(IncompatibleDeviceException::class, CryptoException::class)
    fun getAESKey(): ByteArray? {
        val encodedEncryptedAES = storage.retrieveString(KEY_ALIAS)
        if (!encodedEncryptedAES.isNullOrBlank()) {
            //Return existing key
            val encryptedAES = Base64.decode(encodedEncryptedAES, Base64.DEFAULT)
            val existingAES = rsaDecrypt(encryptedAES)
            val aesExpectedLengthInBytes: Int = AES_KEY_SIZE / 8
            //Prevent returning an 'Empty key' (invalid/corrupted) that was mistakenly saved
            if (existingAES != null && existingAES.size == aesExpectedLengthInBytes) {
                //Key exists and has the right size
                return existingAES
            }
        }
        //Key doesn't exist. Generate new AES
        return try {
            val keyGen = KeyGenerator.getInstance(ALGORITHM_AES)
            keyGen.init(AES_KEY_SIZE)
            val aes = keyGen.generateKey().encoded
            //Save encrypted encoded version
            val encryptedAES = rsaEncrypt(aes)
            val encodedEncryptedAESText = String(Base64.encode(encryptedAES, Base64.DEFAULT))
            storage.store(KEY_ALIAS, encodedEncryptedAESText)
            aes
        } catch (e: NoSuchAlgorithmException) {
            /*
                  * This exceptions are safe to be ignored:
                  *
                  * - NoSuchAlgorithmException:
                  *      Thrown if the Algorithm implementation is not available. AES was introduced in API 1
                  *
                  * Read more in https://developer.android.com/reference/javax/crypto/KeyGenerator
                  */
            Log.e(TAG, "Error while creating the AES key.", e)
            throw IncompatibleDeviceException(e)
        }
    }


    /**
     * Encrypts the given input bytes using a symmetric key (AES).
     * The AES key is stored protected by an asymmetric key pair (RSA).
     *
     * @param encryptedInput the input bytes to decrypt. There's no limit in size.
     * @return the decrypted output bytes
     * @throws CryptoException             if the RSA Key pair was deemed invalid and got deleted. Operation can be retried.
     * @throws IncompatibleDeviceException in the event the device can't understand the cryptographic settings required
     */
    @Throws(CryptoException::class, IncompatibleDeviceException::class)
    fun decrypt(encryptedInput: ByteArray?): ByteArray? {
        return try {
            val key: SecretKey = SecretKeySpec(getAESKey(), ALGORITHM_AES)
            val cipher = Cipher.getInstance(AES_TRANSFORMATION)
            val encodedIV = storage.retrieveString(KEY_IV_ALIAS)
            if (TextUtils.isEmpty(encodedIV)) {
                //AES key was JUST generated. If anything existed before, should be encrypted again first.
                throw CryptoException(
                    "The encryption keys changed recently. You need to re-encrypt something first.",
                    null
                )
            }
            val iv = Base64.decode(encodedIV, Base64.DEFAULT)
            cipher.init(Cipher.DECRYPT_MODE, key, IvParameterSpec(iv))
            cipher.doFinal(encryptedInput)
        } catch (e: NoSuchAlgorithmException) {
            /*
                  * This exceptions are safe to be ignored:
                  *
                  * - NoSuchPaddingException:
                  *      Thrown if NOPADDING is not available. Was introduced in API 1.
                  * - NoSuchAlgorithmException:
                  *      Thrown if the transformation is null, empty or invalid, or if no security provider
                  *      implements it. Was introduced in API 1.
                  * - InvalidKeyException:
                  *      Thrown if the given key is inappropriate for initializing this cipher.
                  * - InvalidAlgorithmParameterException:
                  *      If the IV parameter is null.
                  *
                  * Read more in https://developer.android.com/reference/javax/crypto/Cipher
                  */
            Log.e(TAG, "Error while decrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: NoSuchPaddingException) {
            Log.e(TAG, "Error while decrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: InvalidKeyException) {
            Log.e(TAG, "Error while decrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: InvalidAlgorithmParameterException) {
            Log.e(TAG, "Error while decrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: BadPaddingException) {
            /*
                  * Any of this exceptions mean the encrypted input is somehow corrupted and cannot be recovered.
                  * - BadPaddingException:
                  *      Thrown if the input doesn't contain the proper padding bytes. In this case, if the input contains padding.
                  * - IllegalBlockSizeException:
                  *      Thrown only on encrypt mode.
                  */
            throw CryptoException(
                "The AES encrypted input is corrupted and cannot be recovered. Please discard it.",
                e
            )
        } catch (e: IllegalBlockSizeException) {
            throw CryptoException(
                "The AES encrypted input is corrupted and cannot be recovered. Please discard it.",
                e
            )
        }
    }

    /**
     * Encrypts the given input bytes using a symmetric key (AES).
     * The AES key is stored protected by an asymmetric key pair (RSA).
     *
     * @param decryptedInput the input bytes to encrypt. There's no limit in size.
     * @return the encrypted output bytes
     * @throws CryptoException             if the RSA Key pair was deemed invalid and got deleted. Operation can be retried.
     * @throws IncompatibleDeviceException in the event the device can't understand the cryptographic settings required
     */
    @Throws(CryptoException::class, IncompatibleDeviceException::class)
    fun encrypt(decryptedInput: ByteArray?): ByteArray? {
        return try {
            val key: SecretKey = SecretKeySpec(getAESKey(), ALGORITHM_AES)
            val cipher = Cipher.getInstance(AES_TRANSFORMATION)
            cipher.init(Cipher.ENCRYPT_MODE, key)
            val encrypted = cipher.doFinal(decryptedInput)
            val encodedIV = Base64.encode(cipher.iv, Base64.DEFAULT)
            //Save IV for Decrypt stage
            storage.store(KEY_IV_ALIAS, String(encodedIV))
            encrypted
        } catch (e: NoSuchAlgorithmException) {
            /*
                  * This exceptions are safe to be ignored:
                  *
                  * - NoSuchPaddingException:
                  *      Thrown if NOPADDING is not available. Was introduced in API 1.
                  * - NoSuchAlgorithmException:
                  *      Thrown if the transformation is null, empty or invalid, or if no security provider
                  *      implements it. Was introduced in API 1.
                  * - InvalidKeyException:
                  *      Thrown if the given key is inappropriate for initializing this cipher.
                  * - InvalidAlgorithmParameterException:
                  *      If the IV parameter is null.
                  * - BadPaddingException:
                  *      Thrown only on decrypt mode.
                  *
                  * Read more in https://developer.android.com/reference/javax/crypto/Cipher
                  */
            Log.e(TAG, "Error while encrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: NoSuchPaddingException) {
            Log.e(TAG, "Error while encrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: InvalidKeyException) {
            Log.e(TAG, "Error while encrypting the input.", e)
            throw IncompatibleDeviceException(e)
        } catch (e: IllegalBlockSizeException) {
            /*
                  * - IllegalBlockSizeException:
                  *      Thrown if no padding has been requested and the length is not multiple of block size.
                  * - BadPaddingException:
                  *      Thrown only on decrypt mode.
                  */
            throw CryptoException("The AES decrypted input is invalid.", e)
        } catch (e: BadPaddingException) {
            throw CryptoException("The AES decrypted input is invalid.", e)
        }
    }
}