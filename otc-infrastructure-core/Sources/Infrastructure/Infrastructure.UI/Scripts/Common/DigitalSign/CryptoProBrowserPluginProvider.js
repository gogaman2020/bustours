; (function () {

    //----------------------------------------

    var consts = {
        CADESCOM_CADES_X_LONG_TYPE_1: 0x5d,
        CAPICOM_CURRENT_USER_STORE: 2,
        CAPICOM_MY_STORE: "My",
        CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED: 2,
        CAPICOM_CERTIFICATE_FIND_SUBJECT_NAME: 1,
        CAPICOM_CERTIFICATE_FIND_SHA1_HASH: 0,
        CADES_BES: 1, //не улучшенная эцп
        CADESCOM_CADES_DEFAULT: 0,
        CAPICOM_CERTIFICATE_INCLUDE_CHAIN_EXCEPT_ROOT: 0,
        CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN: 1,
        CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY: 2,
        CADESCOM_STRING_TO_UCS2LE: 0x00, //Данные будут перекодированы в UCS-2 little endian.
        CADESCOM_BASE64_TO_BINARY: 0x01, //Данные будут перекодированы из Base64 в бинарный массив.

        CAPICOM_STORE_OPEN_READ_ONLY: 0,
        MICROSOFT_INTERNET_EXPLORER: 'Microsoft Internet Explorer',
        CAPICOM_ENCODE_BASE64: 0,
        CAPICOM_VERIFY_SIGNATURE_ONLY: 0,

        CAPICOM_INFO_SUBJECT_SIMPLE_NAME: 0,
        CAPICOM_INFO_ISSUER_SIMPLE_NAME: 1,
        CAPICOM_INFO_SUBJECT_EMAIL_NAME: 2,
        CAPICOM_INFO_ISSUER_EMAIL_NAME: 3,

        CADESCOM_HASH_ALGORITHM_CP_GOST_3411: 100,
        CADESCOM_HASH_ALGORITHM_CP_GOST_3411_2012_256: 101,
        CADESCOM_HASH_ALGORITHM_SHA1: 0,

        MIN_VERSION: {
            Major: 1,
            Minor: 5,
            Build: 1500
        },

        MIN_VERSION_FOR_CHROME_45: {
            Major: 2,
            Minor: 0,
            Build: 12235
        }
    };

    //----------------------------------------

    var chromeVersion = (function() {
        var raw = navigator.userAgent.match(/Chrom(e|ium)\/([0-9]+)\./);

        return raw ? parseInt(raw[2], 10) : false;
    })();

    if (chromeVersion && chromeVersion >= 45) {
        consts.MIN_VERSION = consts.MIN_VERSION_FOR_CHROME_45;
    }

    //----------------------------------------

    var resolveProvider, rejectProvider;

    var provider = new Promise(function (resolve, reject) {
        resolveProvider = resolve;
        rejectProvider = reject;
    });

    //----------------------------------------

    function stub() {
        throw new Error('Не создан объект КриптоПро ЭЦП Browser plug-in. Не установлен КриптоПро ЭЦП Browser plug-in. Установите и проверьте плагин.');
    }

    var providerStub = {
        ValidateProvider: stub,
        CertCount: stub,
        CertByHash: stub,
        CertById: stub,
        SignString: stub,
        SignBase64: stub,
        SignHash: stub
    };

    //----------------------------------------

    function isCorrectVersionInstalled() {
        return provider.then(function(impl) {
                return impl.IsCorrectVersionInstalled();
            },
            function() {
                return false;
            });
    }

    function showWarning() {
        //$('#cadeswarning').modal();

        var url = 'https://www.otc.ru/portals/0/Files/cadesplugin.exe';
        if (window.location.hostname == 'msp.lot-online.ru') {
            url = 'https://msp.lot-online.ru/portals/0/Files/cadesplugin.exe';
        }

        var ww = window.open('', '', 'width=600,height=200');
        ww.document.writeln('<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN"> \
        <HTML>\
            <HEAD>\
                <META content="text/html; charset=unicode" http-equiv=Content-Type>\
                <META name=GENERATOR content="MSHTML 8.00.7600.16700">\
                <META HTTP-EQUIV="REFRESH" CONTENT="2;URL=' + url + '">\
            </HEAD>\
            <BODY onload="window.location.reload();">\
                <P align=center><STRONG>Для корректной работы с сайтом рекомендуется скачать и установить плагин КриптоПро ЭЦП (CADESCOM) версии 2.0.12245</STRONG></P>\
                <p align=center>Если загрузка файла не началась автоматически, скачайте его <A href="' + url + '">по этой ссылке.</A></P>\
                <p align="center">\
                    <br />\
		            При возникновении проблем во время установки, закройте браузер и нажмите Повторить.\
                </p>\
            </BODY>\
        </HTML>\
        <script>document.location.href="' + url + '";</script>');
    }

    function createProvider() {
        var instanceImpl = provider.then(function(impl) {
            return new impl();
        }, function () {
            showWarning();
            return providerStub;
        });

        function createDelegate(methodName) {
            return function () {
                var args = arguments;
                return instanceImpl.then(function (self) {
                    return self[methodName].apply(self, args);
                });
            };
        }

        var instance = {
            ValidateProvider: null,
            CertCount: null,
            CertByHash: null,
            CertById: null,
            SignString: null,
            SignBase64: null,
            SignHash: null
        };

        for (var methodName in instance) {
            if (instance.hasOwnProperty(methodName)) {
                instance[methodName] = createDelegate(methodName);
            }
        }

        return instance;
    }

    //----------------------------------------

    window.CryptoProBrowserPluginProvider = {
        Consts: consts,
        ShowWarning: showWarning,
        ResolveProvider: resolveProvider,
        RejectProvider: rejectProvider,
        IsCorrectVersionInstalled: isCorrectVersionInstalled,
        CreateProvider: createProvider
    };

    //----------------------------------------

    function getTimestamp() {
        function pad(number) {
            if (number < 10) {
                return '0' + number;
            }
            return number;
        }
        var now = new Date();
        var timestamp = now.getUTCFullYear() +
            '-' + pad(now.getUTCMonth() + 1) +
            '-' + pad(now.getUTCDate()) +
            'T' + pad(now.getUTCHours()) +
            ':' + pad(now.getUTCMinutes()) +
            ':' + pad(now.getUTCSeconds()) +
            '.' + (now.getUTCMilliseconds() / 1000).toFixed(3).slice(2, 5) +
            'Z';
      return timestamp;
    }

    cadesplugin.then(function() {
            var isAsync = !!cadesplugin.CreateObjectAsync;

            try {
                var thisScriptSrc = $('script[src*="CryptoProBrowserPluginProvider.js"]').attr('src');
                var baseUrl = thisScriptSrc.slice(0, thisScriptSrc.indexOf('CryptoProBrowserPluginProvider.js'));

                var script = document.createElement('script');

                if (isAsync) {
                    script.setAttribute('src', baseUrl + 'CryptoProBrowserPluginProviderAsync.js?' + getTimestamp());
                } else {
                    script.setAttribute('src', baseUrl + 'CryptoProBrowserPluginProviderSync.js?' + getTimestamp());
                }

                document.getElementsByTagName('head')[0].appendChild(script);
            } catch (e) {
            }
        },
        function(err) {
            rejectProvider(err);
        });

})();
