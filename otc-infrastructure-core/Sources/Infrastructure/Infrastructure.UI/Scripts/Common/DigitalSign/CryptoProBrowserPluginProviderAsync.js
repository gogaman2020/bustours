;(function() {
     
    var consts = CryptoProBrowserPluginProvider.Consts;

    function CryptoProBrowserPluginProvider_Async() {
        var self = this;

        self.Store = new Promise(function(resolve, reject) {
            self._resolveStore = resolve;
            self._rejectStore = reject;
        });

        self.Signer = new Promise(function(resolve, reject) {
            self._resolveSigner = resolve;
            self._rejectSigner = reject;
        });

        cadesplugin.async_spawn(function* () {
            var isCorrect = yield CryptoProBrowserPluginProvider_Async.IsCorrectVersionInstalled();

            if (isCorrect) {
                yield self.CreateProvider();
            } else {
                CryptoProBrowserPluginProvider.ShowWarning();
            }
        });
    }

    //static
    CryptoProBrowserPluginProvider_Async.IsCorrectVersionInstalled = function () {
        return cadesplugin.async_spawn(function* () {
            yield cadesplugin;

            try {
                var about = yield cadesplugin.CreateObjectAsync('CAdESCOM.About');
            } catch(err) {
                return false;
            }

            try {
                var version = {
                    Major: yield about.MajorVersion,
                    Minor: yield about.MinorVersion,
                    Build: yield about.BuildVersion
                }
            } catch (err) {
                return false;
            }

            var minVersion = consts.MIN_VERSION;

            return (version.Major > minVersion.Major)
                || (version.Major === minVersion.Major && version.Minor > minVersion.Minor)
                || (version.Major === minVersion.Major && version.Minor === minVersion.Minor && version.Build >= minVersion.Build);
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.CreateObject = function (name) {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            yield self.ValidateProvider();

            try {
                var object = yield cadesplugin.CreateObjectAsync(name);

                return object;
            } catch (err) {}
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.CreateProvider = function () {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            // Создание объектов КриптоПро ЭЦП Browser plug-in

            try {
                var store = yield self.CreateObject('CAPICOM.Store');
                self._resolveStore(store);
            } catch (err) {
                self._rejectSigner(err);
            }

            try {
                var signer = yield self.CreateObject('CAdESCOM.CPSigner');
                self._resolveSigner(signer);
            } catch (err) {
                self._rejectSigner(err);
            }
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.ValidateProvider = function () {
        return cadesplugin.async_spawn(function* () {
            var isCorrect = yield CryptoProBrowserPluginProvider_Async.IsCorrectVersionInstalled();

            if (!isCorrect) {
                throw new Error('Не создан объект КриптоПро ЭЦП Browser plug-in. Не установлен КриптоПро ЭЦП Browser plug-in. Установите и проверьте плагин.');
            }
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.CertCount = function () {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            try {
                var store = yield self.Store;
                yield store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

                var certificates = yield store.Certificates;

                var certificatesCount = yield certificates.Count;

                return certificatesCount;
            } catch (e) {
                return 0;
            }
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.CertByHash = function (certHash) {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            var store = yield self.Store;
            yield store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

            var certificates = yield store.Certificates;
            certificates = yield certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, certHash);

            if (certificates.Count == 0) {
                throw new Error('Certificate not found');
            }

            var certificate = yield certificates.Item(1);

            var subject = yield certificate.SubjectName;

            var firstNameMatch, surnameMatch, middleNameMatch;

            if (subject.match(/G\s*=/)) {
                firstNameMatch = subject.match(/G\s*=[\s"]*([а-яА-ЯёЁ\w-.]+)/);
                surnameMatch = subject.match(/SN\s*=[\s"]*([а-яА-ЯёЁ\w-.]+)/);
                middleNameMatch = subject.match(/G\s*=[\s"]*[а-яА-ЯёЁ\w-.]+\s+(([а-яА-ЯёЁ\w-.\s]+))/);
            } else {
                firstNameMatch = subject.match(/CN\s*=[\s"]*[а-яА-ЯёЁ\w-.]+\s+([а-яА-ЯёЁ\w-.]+)/);
                surnameMatch = subject.match(/CN\s*=[\s"]*([а-яА-ЯёЁ\w-.]+)/);
                middleNameMatch = subject.match(/CN\s*=[\s"]*[а-яА-ЯёЁ\w-.]+\s+[а-яА-ЯёЁ\w-.]+\s+(([а-яА-ЯёЁ\w-.\s]+))/);
            }

            var names = [surnameMatch, firstNameMatch, middleNameMatch];
            for (var j = 0; j < names.length; j++) {
                names[j] = (names[j] && names[j][1]) ? names[j][1] : null;
            }

            var organization = '';
            if (subject.match(/O\s*=/)) {
                organization = subject.match(/O\s*=[\s"]*[^,]+/)[0];
                var startPosEq = organization.search('=');
                organization = organization.substring(startPosEq + 1, organization.length - startPosEq + 2);
            }

            var validAfter = yield certificate.ValidFromDate;
            var validBefore = yield certificate.ValidToDate;
            var issuer = yield certificate.GetInfo(consts.CAPICOM_INFO_ISSUER_SIMPLE_NAME);
            var hash = yield certificate.Thumbprint;
            var publicKey = yield certificate.PublicKey();
            var algorithm = yield publicKey.Algorithm;
            var algCode = yield algorithm.Value;

            return {
                ValidAfter: new Date(validAfter),
                ValidBefore: new Date(validBefore),
                Subject: names.join(' '),
                Issuer: issuer,
                Hash: hash,
                Organization: organization,
                Algorithm: algCode
            };
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.CertById = function (i) {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            var store = yield self.Store;
            yield store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

            var certificates = yield store.Certificates;

            var certificate = yield certificates.Item(i + 1);

            var subject = yield certificate.SubjectName;

            var firstNameMatch, surnameMatch, middleNameMatch;

            if (subject.match(/G\s*=/)) {
                firstNameMatch = subject.match(/G\s*=[\s"]*([а-яА-ЯёЁ\w-.]+)/);
                surnameMatch = subject.match(/SN\s*=[\s"]*([а-яА-ЯёЁ\w-.]+)/);
                middleNameMatch = subject.match(/G\s*=[\s"]*[а-яА-ЯёЁ\w-.]+\s+(([а-яА-ЯёЁ\w-.\s]+))/);
            } else {
                firstNameMatch = subject.match(/CN\s*=[\s"]*[а-яА-ЯёЁ\w-.]+\s+([а-яА-ЯёЁ\w-.]+)/);
                surnameMatch = subject.match(/CN\s*=[\s"]*([а-яА-ЯёЁ\w-.]+)/);
                middleNameMatch = subject.match(/CN\s*=[\s"]*[а-яА-ЯёЁ\w-.]+\s+[а-яА-ЯёЁ\w-.]+\s+(([а-яА-ЯёЁ\w-.\s]+))/);
            }

            var names = [surnameMatch, firstNameMatch, middleNameMatch];
            for (var j = 0; j < names.length; j++) {
                names[j] = (names[j] && names[j][1]) ? names[j][1] : null;
            }

            var organization = '';
            if (subject.match(/O\s*=/)) {
                organization = subject.match(/O\s*=[\s"]*[^,]+/)[0];
                var startPosEq = organization.search('=');
                organization = organization.substring(startPosEq + 1, organization.length - startPosEq + 2);
            }

            var validAfter = yield certificate.ValidFromDate;
            var validBefore = yield certificate.ValidToDate;
            var issuer = yield certificate.GetInfo(consts.CAPICOM_INFO_ISSUER_SIMPLE_NAME);
            var hash = yield certificate.Thumbprint;
            var publicKey = yield certificate.PublicKey();
            var algorithm = yield publicKey.Algorithm;
            var algCode = yield algorithm.Value;

            return {
                ValidAfter: new Date(validAfter),
                ValidBefore: new Date(validBefore),
                Subject: names.join(' '),
                Issuer: issuer,
                Hash: hash,
                Organization: organization,
                Algorithm: algCode
            };
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.SignString = function (toSign, currentCertificate, isDetached) {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            var store = yield self.Store;
            yield store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

            var certificates = yield store.Certificates;
            certificates = yield certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, currentCertificate.Hash);

            if (certificates.Count == 0) {
                throw new Error('Certificate not found');
            }

            var certificate = yield certificates.Item(1);

            var signer = yield self.Signer;
            yield signer.propset_Certificate(certificate);
            yield signer.propset_Options(consts.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY);

            var signedData = yield self.CreateObject('CAdESCOM.CadesSignedData');
            yield signedData.propset_Content(toSign);

            try {
                var signedMessage = yield signedData.SignCades(signer, consts.CADES_BES, isDetached);
            } catch (err) {
                throw new Error('Failed to create signature. Error: ' + getErrorMessage(err));
            }

            yield store.Close();

            return signedMessage;
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.SignBase64 = function (toSign, currentCertificate, isDetached) {
        var self = this;

        return cadesplugin.async_spawn(function* () {
            var store = yield self.Store;
            yield store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

            var certificates = yield store.Certificates;
            certificates = yield certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, currentCertificate.Hash);

            if (certificates.Count == 0) {
                throw new Error('Certificate not found');
            }

            var certificate = yield certificates.Item(1);

            var signer = yield self.Signer;
            yield signer.propset_Certificate(certificate);
            yield signer.propset_Options(consts.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY);

            var signedData = yield self.CreateObject('CAdESCOM.CadesSignedData');
            yield signedData.propset_ContentEncoding(consts.CADESCOM_BASE64_TO_BINARY);
            yield signedData.propset_Content(toSign);

            try {
                var signedMessage = yield signedData.SignCades(signer, consts.CADES_BES, isDetached);
            } catch (err) {
                throw new Error('Failed to create signature. Error: ' + getErrorMessage(err));
            }

            yield store.Close();

            return signedMessage;
        });
    };

    CryptoProBrowserPluginProvider_Async.prototype.SignHash = function(hashToSign, currentCertificate, hashAlgorithm) {
        if (typeof hashAlgorithm === 'undefined' || hashAlgorithm === null) {
            throw new Error('Hash alrorithm is missing');
        }
        
        var self = this;

        return cadesplugin.async_spawn(function* () {
            var store = yield self.Store;
            yield store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

            var certificates = yield store.Certificates;
            certificates = yield certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, currentCertificate.Hash);

            if (certificates.Count == 0) {
                throw new Error('Certificate not found');
            }

            var certificate = yield certificates.Item(1);

            var signer = yield self.Signer;
            yield signer.propset_Certificate(certificate);
            yield signer.propset_Options(consts.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY);

            var hashedData = yield self.CreateObject('CAdESCOM.HashedData');
      
            yield hashedData.propset_Algorithm(hashAlgorithm);
            yield hashedData.SetHashValue(hashToSign);

            var signedData = yield self.CreateObject('CAdESCOM.CadesSignedData');

            try {
                var signedMessage = yield signedData.SignHash(hashedData, signer, consts.CADES_BES);
            } catch (err) {
                throw new Error('Failed to create signature. Error: ' + getErrorMessage(err));
            }

            yield store.Close();

            return signedMessage;
        });
    };

    function getErrorMessage(e) {
        var err = e.message;
        if (!err) {
            err = e;
        } else if (e.number) {
            err += " (0x" + decimalToHexString(e.number) + ")";
        }
        return err;
    }

    function decimalToHexString(number) {
        if (number < 0) {
            number = 0xFFFFFFFF + number + 1;
        }
        return number.toString(16).toUpperCase();
    };

    CryptoProBrowserPluginProvider.ResolveProvider(CryptoProBrowserPluginProvider_Async);

})();
