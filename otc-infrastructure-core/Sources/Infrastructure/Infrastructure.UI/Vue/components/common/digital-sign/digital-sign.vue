<template>
    <div class="digital-sign"> 
            <form @submit.prevent="signAll" :class="style.form">
                <loading v-if="isLoading">
                    Пожалуйста, подождите...
                </loading>
                <table class="table table-striped table-hover certificateTable">
                    <thead>
                        <tr>
                            <th>Сертификат</th>
                            <th>Срок действия</th>
                            <th>УЦ</th>
                            <th>Организация</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="cert in computedCertificates" :class="{ selectedRow: selectedCertificate && cert === selectedCertificate }" @click="onCertificateRowClick(cert)">
                            <td>{{ cert.Subject }}</td>
                            <td>{{ cert.ExpiresCellValue }}</td>
                            <td>{{ cert.Issuer }}</td>
                            <td>{{ cert.Organization }}</td>
                        </tr>
                    </tbody>
                </table>
                <div :class="style.formActions">
                    <button-blue v-if="selectedCertificate" type="submit">Выбрать</button-blue>
                    <button-green type="button" @click="$emit('close-modal')">Отмена</button-green>
                </div>
            </form>
        </div>
</template>

<script>
    import style from './style.module.scss'
    import axios from 'axios';

    require('es6-promise').polyfill();
    require('babel-polyfill');
    require('../../../../Scripts/Common/cadesplugin_api.js');
    require('../../../../Scripts/Common/DigitalSign/CryptoProBrowserPluginProvider.js');
    require('../../../../Scripts/Common/DigitalSign/CryptoProBrowserPluginProvider' + (typeof cadesplugin !== 'undefined' && cadesplugin && !!cadesplugin.CreateObjectAsync ? 'Async' : 'Sync') + '.js');

    import buttonBlue from 'EXT/components/controls/buttons/button-blue'
    import buttonGreen from 'EXT/components/controls/buttons/button-green'
    import loading from 'EXT/components/common/loading/loading'

    export default {
        props: {
            filterGost: String, //ОИДы через запятую.
            testCertificateThumbprint: String,
            currentCertificateThumbprint: String,
            contentForSign: String
        },
        components: {
            buttonBlue,
            buttonGreen,
            loading
        },
        data() {
            return {
                style: style,
                cryptoProvider: null,
                certificates: [],
                isCertificatesLoaded: false,
                selectedCertificate: null,
                singOfObject: null,
                singOfFiles: null,
                hashAlgorithms: {
                    CADESCOM_HASH_ALGORITHM_CP_GOST_3411: 100,
                    CADESCOM_HASH_ALGORITHM_CP_GOST_3411_2012_256: 101,
                    CADESCOM_HASH_ALGORITHM_SHA1: 0
                },
                keyExportImportAlgorithms: {
                    Gost2001: '1.2.643.2.2.19',
                    Gost2012_256: '1.2.643.7.1.1.1.1'
                },
                isLoading: false,
                dataForSign: this.contentForSign
            };
        },
        computed: {
            computedCertificates() {
                if (!this.certificates || this.certificates.length == 0) {
                    return this.certificates;
                }

                var algorithmOids = this.filterGost
                    ? this.filterGost.split(',')
                    : null;
                var curDate = new Date();
                var result = this.certificates.filter(cert => {
                    //Проверка активности сертификата.
                    var isValid = curDate > cert.ValidAfter && curDate < cert.ValidBefore;

                    //Проверка соответствия ГОСТу.
                    if (isValid && algorithmOids) {
                        isValid = algorithmOids.indexOf(cert.Algorithm) !== -1 ||
                            (this.testCertificateThumbprint && cert.Hash.localeCompare(this.testCertificateThumbprint) == 0);
                    }

                    return isValid;
                });

                return result;
            },
        },
        mounted(){
            if (!this.isCertificatesLoaded) {
                this.loadCertificates();
            }
        },
        methods: {
            //Private.
            loadCertificates() {
                var self = this;
                self.isLoading = true;
                this.getAndValidateCrypto()
                    .then(function (provider) {
                        self.cryptoProvider = provider;

                        return self.cryptoProvider
                            .CertCount()
                            .then(function (certCount) {
                                var certs = [];
                                for (var i = 0; i < certCount; ++i) {
                                    certs.push(self.cryptoProvider
                                        .CertById(i)
                                        .then(function (cert) {
                                            //Если юзер зашел по сертификату, то в списке должен быть только этот сертификат.
                                            if ((self.currentCertificateThumbprint && cert.Hash.localeCompare(self.currentCertificateThumbprint) == 0) ||
                                                !self.currentCertificateThumbprint) {
                                                self.certificates.push(self.getCertificateInfo(cert));
                                            }
                                        }));
                                }
                                self.isCertificatesLoaded = true;

                                return Promise.all(certs);
                            }).then(() => {
                                if (self.certificates.length == 1) {
                                    self.selectedCertificate = self.certificates[0];
                                }
                            });
                    })
                    .finally(() => self.isLoading = false);
            },
            getCertificateInfo(certificate) {
                certificate['ExpiresCellValue'] = this.getCertExpires(certificate);
                return certificate;
            },
            padZero(str, cnt) {
                var rc = str.toString();
                while (rc.length < cnt) {
                    rc = "0" + rc;
                }

                return rc;
            },
            dateShortToString(dt) {
                var rc = new Date(dt);
                return this.padZero(rc.getDate(), 2) + "." + this.padZero(rc.getMonth() + 1, 2) + "." + rc.getFullYear();
            },
            getCertExpires(certificate) {
                var dateStr = this.dateShortToString(certificate.ValidAfter) + ' - ' + this.dateShortToString(certificate.ValidBefore);
                var curDate = new Date();
                var enabled = curDate > certificate.ValidAfter && curDate < certificate.ValidBefore;
                if (!enabled) {
                    dateStr += ' (не активный)';
                }

                return dateStr;
            },
            getAndValidateCrypto() {
                var cryptoProvider = CryptoProBrowserPluginProvider.CreateProvider();
                return cryptoProvider.ValidateProvider()
                    .then(function () {
                        return cryptoProvider;
                    });
            },
            onCertificateRowClick(certificate) {
                this.selectedCertificate = certificate;
            },
            signObject() {
                if (!this.selectedCertificate) {
                    return;
                }

                if (!this.validateCertificateDates(this.selectedCertificate)) {
                    bootbox.alert('Срок действия сертификата либо не вступил в силу, либо истек. Подпись не может быть использована.');
                } else {
                    this.signAfterCertAccept();
                }  
            },
            validateCertificateDates(certificate) {
                var curDate = new Date();
                return curDate > certificate.ValidAfter && curDate < certificate.ValidBefore;
            },
            signAfterCertAccept() {
                try {
                    this.signAll();
                } catch (e) {
                    bootbox.alert({
                        title: 'Ошибка при подписании',
                        message: this.resolveErrorMessage(e)
                    });
                }
            },
            resolveErrorMessage(er) {
                if (typeof (er) == "string") {
                    return er;
                } else if (typeof (er) == "object" && er.message) {
                    return er.message;
                }
                else {
                    return "Неопределенная ошибка";
                }
            },
            signAll() {
                this.isLoading = true;
                this.prepareProgress()
                    .then(response => {
                        var result = this.sign64(this.dataForSign)
                            .then(sign => {
                                this.singOfObject = sign;
                                return Promise.resolve();
                            });

                        return result;
                    })
                    .then(() => {
                        var obj = {
                            DataForSign: this.dataForSign,
                            DigitalSign: this.singOfObject,
                            FileDigitalSigns: this.signOfFiles,
                            FilterGostOnly: this.filterGost && this.filterGost.length > 0,
                        };
                        this.clearTempData();
                        this.$emit('onDataSigned', obj);
                        return Promise.resolve();
                    })
                    .catch(error => {
                        this.isLoading = false;

                        if (error.isHandled !== true) {
                            console.log(error);
                            bootbox.alert({
                                title: 'Ошибка при подписании',
                                message: error.message
                            });
                        }
                    })
                    .finally(() => this.isLoading = false);
            },
            prepareProgress() {
                return new Promise((resolve, reject) => {
                    //Показать диалог модальный на котором будет прогресс бар
                    //Стартануть прогрес бар
                    resolve();
                });
            },
            sign64(contentForSign) {
                return this.cryptoProvider.SignBase64(contentForSign, this.selectedCertificate, true);
            },
            signAllFiles(files) {
                var result = Promise.resolve()

                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    result = result.then(() => {
                        return this.signFile(file);
                    });
                }

                return result.then(() => {
                    this.signOfFiles = files.map(file => {
                        var res = {
                            key: file.Guid,
                            value: file.DigitalSign,
                        };

                        return res;
                    });
                });
            },
            signFile(file) {
                var self = this;
                var cert = this.selectedCertificate;

                var getFileHashGost = function (file) {
                    if (cert.Algorithm === self.keyExportImportAlgorithms.Gost2001) {
                        return file.HashGost94;
                    }
                    if (cert.Algorithm === self.keyExportImportAlgorithms.Gost2012_256) {
                        return file.HashGost2012_256;
                    }
                    return file.HashSha1;
                };

                var getHashGostAlgorithm = function () {
                    if (cert.Algorithm === self.keyExportImportAlgorithms.Gost2001) {
                        return self.hashAlgorithms.CADESCOM_HASH_ALGORITHM_CP_GOST_3411;
                    }
                    if (cert.Algorithm === self.keyExportImportAlgorithms.Gost2012_256) {
                        return self.hashAlgorithms.CADESCOM_HASH_ALGORITHM_CP_GOST_3411_2012_256;
                    }
                    return self.hashAlgorithms.CADESCOM_HASH_ALGORITHM_SHA1;
                };

                var fileHashGost = getFileHashGost(file);
                var result;

                if (typeof fileHashGost === "string" && fileHashGost.length > 0) {
                    var hashGostAlgorithm = getHashGostAlgorithm();
                    result = this.signHash(fileHashGost, hashGostAlgorithm)
                        .then(sign => {
                            file.DigitalSign = sign;
                        })
                        .then(() => {
                            if (!(typeof file.DigitalSign === "string" && file.DigitalSign.length > 0)) {
                                return this.signHash(file.HashSha1, hashAlgorithms.CADESCOM_HASH_ALGORITHM_SHA1)
                                    .then(sign => {
                                        file.DigitalSign = sign;
                                    });
                            } else {
                                return null;
                            }
                        });
                } else {
                    result = Promise.resolve();
                }

                result.then(() => {
                    if (typeof file.DigitalSign === "string" && file.DigitalSign.length > 0) {
                        return Promise.resolve();
                    } else {
                        
                    }
                });

                return result;
            },
            signHash(hashToSign, hashAlgorithm) {
                return this.cryptoProvider.SignHash(hashToSign, this.selectedCertificate, hashAlgorithm);
            },
            clearTempData() {
                this.singOfObject = null;
                this.singOfFiles = null;
            },
        },
    };
</script>