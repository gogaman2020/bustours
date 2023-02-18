;(function() {
     
    var consts = CryptoProBrowserPluginProvider.Consts;

    function CryptoProBrowserPluginProvider_Sync() {
        var self = this;

        self.Store = null;
        self.Signer = null;

        if (CryptoProBrowserPluginProvider_Sync.IsCorrectVersionInstalled_Sync()) {
            self.CreateProvider_Sync();
        } else {
            CryptoProBrowserPluginProvider.ShowWarning();
        }
    }

    function wrap_sync(methodName) {
        return function() {
            var self = this;
            var args = arguments;

            return new Promise(function(resolve) {
                var result = self[methodName].apply(self, args);
                resolve(result);
            });
        };
    }

    CryptoProBrowserPluginProvider_Sync.IsCorrectVersionInstalled = wrap_sync('IsCorrectVersionInstalled_Sync');

    CryptoProBrowserPluginProvider_Sync.prototype.ValidateProvider = wrap_sync('ValidateProvider_Sync');
    CryptoProBrowserPluginProvider_Sync.prototype.CertCount = wrap_sync('CertCount_Sync');
    CryptoProBrowserPluginProvider_Sync.prototype.CertByHash = wrap_sync('CertByHash_Sync');
    CryptoProBrowserPluginProvider_Sync.prototype.CertById = wrap_sync('CertById_Sync');
    CryptoProBrowserPluginProvider_Sync.prototype.SignString = wrap_sync('SignString_Sync');
    CryptoProBrowserPluginProvider_Sync.prototype.SignBase64 = wrap_sync('SignBase64_Sync');
    CryptoProBrowserPluginProvider_Sync.prototype.SignHash = wrap_sync('SignHash_Sync');

    //staticH
    CryptoProBrowserPluginProvider_Sync.IsCorrectVersionInstalled_Sync = function () {
        var minVersion = consts.MIN_VERSION;

        var cadescomAbout = null;
        try {
            cadescomAbout = cadesplugin.CreateObject('CAdESCOM.About');
        } catch (err) { }

        if (cadescomAbout) {
            return (cadescomAbout.MajorVersion > minVersion.Major) ||
                (cadescomAbout.MajorVersion === minVersion.Major && cadescomAbout.MinorVersion > minVersion.Minor) ||
                (cadescomAbout.MajorVersion === minVersion.Major && cadescomAbout.MinorVersion === minVersion.Minor && cadescomAbout.BuildVersion >= minVersion.Build);
        } else {
            return false;
        }
    };

    CryptoProBrowserPluginProvider_Sync.prototype.CreateObject_Sync = function (name) {
        var self = this;
        self.ValidateProvider_Sync();

        try {
            var cadesObject = cadesplugin.CreateObject(name);
            return cadesObject;
        } catch (err) { }
    };

    CryptoProBrowserPluginProvider_Sync.prototype.CreateProvider_Sync = function () {
        var self = this;
        // Создание объектов КриптоПро ЭЦП Browser plug-in
        self.Store = self.CreateObject_Sync('CAPICOM.Store');
        self.Signer = self.CreateObject_Sync('CAdESCOM.CPSigner');
    };

    CryptoProBrowserPluginProvider_Sync.prototype.ValidateProvider_Sync = function () {
        if (!CryptoProBrowserPluginProvider_Sync.IsCorrectVersionInstalled_Sync()) {
            throw new Error('Не создан объект КриптоПро ЭЦП Browser plug-in. Не установлен КриптоПро ЭЦП Browser plug-in. Установите и проверьте плагин.');
        }
    };

    CryptoProBrowserPluginProvider_Sync.prototype.SignString_Sync = function (toSign, currentCertificat, isDetached) {
        var self = this;

        self.Store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
        var oCertificates = self.Store.Certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, currentCertificat.Hash);
        if (oCertificates.Count == 0) {
            throw new Error('Certificate not found');
        }

        var oCertificate = oCertificates.Item(1);
        self.Signer.Certificate = oCertificate;
        //oSigner.TSAAddress = 'http://cryptopro.ru/tsp/';

        var oSignedData = self.CreateObject_Sync('CAdESCOM.CadesSignedData');
        oSignedData.Content = toSign;
        self.Signer.Options = consts.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY;
        try {
            var sSignedMessage = oSignedData.SignCades(self.Signer, consts.CADES_BES, isDetached);
        } catch (err) {
            throw new Error('Failed to create signature. Error: ' + self.GetErrorMessage(err));
        }

        self.Store.Close();

        return sSignedMessage;
    };

    CryptoProBrowserPluginProvider_Sync.prototype.SignBase64_Sync = function (toSign, currentCertificat, isDetached) {
        var self = this;

        self.Store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
        var oCertificates = self.Store.Certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, currentCertificat.Hash);
        if (oCertificates.Count == 0) {
            throw new Error('Certificate not found');
        }

        var oCertificate = oCertificates.Item(1);
        self.Signer.Certificate = oCertificate;
        //oSigner.TSAAddress = 'http://cryptopro.ru/tsp/';

        var oSignedData = self.CreateObject_Sync('CAdESCOM.CadesSignedData');
        oSignedData.ContentEncoding = consts.CADESCOM_BASE64_TO_BINARY;
        oSignedData.Content = toSign;
        self.Signer.Options = consts.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY;
        try {
            var sSignedMessage = oSignedData.SignCades(self.Signer, consts.CADES_BES, isDetached);
        } catch (err) {
            throw new Error('Failed to create signature. Error: ' + self.GetErrorMessage(err));
        }

        self.Store.Close();

        return sSignedMessage;
    };

    //private
    CryptoProBrowserPluginProvider_Sync.prototype.InitializeHashedData = function (sHashValue, hashAlg) {
        var self = this;
        // Создаем объект CAdESCOM.HashedData
        var oHashedData = self.CreateObject_Sync('CAdESCOM.HashedData');

        // Инициализируем объект заранее вычисленным хэш-значением
        // Алгоритм хэширования нужно указать до того, как будет передано хэш-значение
        oHashedData.Algorithm = hashAlg;
        oHashedData.SetHashValue(sHashValue);

        return oHashedData;
    };

    CryptoProBrowserPluginProvider_Sync.prototype.SignHash_Sync = function(hashToSign, currentCertificate, hashAlgorithm) {
        if (typeof hashAlgorithm === 'undefined' || hashAlgorithm === null) {
            throw new Error('Hash alrorithm is missing');
        }
        
        var self = this;

        self.Store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
        var oCertificates = self.Store.Certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, currentCertificate.Hash);
        if (oCertificates.Count == 0) {
            throw new Error('Certificate not found');
        }

        var oCertificate = oCertificates.Item(1);
        self.Signer.Certificate = oCertificate;

        // Создаем объект CAdESCOM.HashedData
        var oHashedData = self.InitializeHashedData(hashToSign, hashAlgorithm);

        var oSignedData = self.CreateObject_Sync('CAdESCOM.CadesSignedData');
        self.Signer.Options = consts.CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY;
        try {
            var sSignedMessage = oSignedData.SignHash(oHashedData, self.Signer, consts.CADES_BES);
        } catch (err) {
            throw new Error('Failed to create signature. Error: ' + self.GetErrorMessage(err));
        }

        self.Store.Close();

        return sSignedMessage;
    };

    CryptoProBrowserPluginProvider_Sync.prototype.CertCount_Sync = function () {
        var self = this,
            result;
        try {
            self.Store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
            result = self.Store.Certificates.Count;
        } catch (e) {
            result = 0;
        }
        return result;
    };

    CryptoProBrowserPluginProvider_Sync.prototype.CertByHash_Sync = function (certHash) {
        var self = this;

        self.Store.Open(consts.CAPICOM_CURRENT_USER_STORE, consts.CAPICOM_MY_STORE, consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
        var oCertificates = self.Store.Certificates.Find(consts.CAPICOM_CERTIFICATE_FIND_SHA1_HASH, certHash);
        if (oCertificates.Count == 0) {
            throw new Error('Certificate not found');
        }

        var cert = oCertificates.Item(1);

        var subject = cert.SubjectName,
            firstNameMatch, surnameMatch, middleNameMatch;
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

        return {
            ValidAfter: cert.ValidFromDate,
            ValidBefore: cert.ValidToDate,
            Subject: names.join(' '),
            Issuer: cert.GetInfo(consts.CAPICOM_INFO_ISSUER_SIMPLE_NAME),
            Hash: cert.Thumbprint,
            Organization: organization,
            Algorithm: cert.PublicKey().Algorithm.Value
        };
    };

    CryptoProBrowserPluginProvider_Sync.prototype.CertById_Sync = function (i) {
        var self = this;
        self.Store.Open(
            consts.CAPICOM_CURRENT_USER_STORE,
            consts.CAPICOM_MY_STORE,
            consts.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);
        var cert = self.Store.Certificates.Item(i + 1); //нумерация с 1 а не с 0 как обычно

        var subject = cert.SubjectName,
            firstNameMatch, surnameMatch, middleNameMatch;
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

        return {
            ValidAfter: cert.ValidFromDate,
            ValidBefore: cert.ValidToDate,
            Subject: names.join(' '),
            Issuer: cert.GetInfo(consts.CAPICOM_INFO_ISSUER_SIMPLE_NAME),
            Hash: cert.Thumbprint,
            Organization: organization,
            Algorithm: cert.PublicKey().Algorithm.Value
        };
    };

    CryptoProBrowserPluginProvider_Sync.prototype.GetErrorMessage = function (e) {
        var err = e.message;
        if (!err) {
            err = e;
        } else if (e.number) {
            err += ' (0x' + this.decimalToHexString(e.number) + ')';
        }
        return err;
    };

    CryptoProBrowserPluginProvider_Sync.prototype.decimalToHexString = function (number) {
        if (number < 0) {
            number = 0xFFFFFFFF + number + 1;
        }

        return number.toString(16).toUpperCase();
    };

    CryptoProBrowserPluginProvider.ResolveProvider(CryptoProBrowserPluginProvider_Sync);
})();
