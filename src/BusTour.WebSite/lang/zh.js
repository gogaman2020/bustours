export default {
    yes: 'Yes',
    no: 'No',
    steps: {
        TourOrderNotPaidStep: 'hghghgh'
    },
    enums: {
        GiftCertificateStatus: {
            Draft: 'Draft',
            Active: 'Active',
            Redeemed: 'Redeemed',
            Expired: 'Expired',
            Сancelled: 'Canceled'
        },
        TourType: {
            Regular: 'Regular',
            RegularGroup: 'Group',
            PrivateHire: 'Private hire',
            Service: 'Service'
        },
        OrderType: {
            Regular: 'Regular',
            RegularGroup: 'Group',
            PrivateHire: 'Private hire',
            Service: 'Service'
        },
        TourState: {
            Draft: "Draft",
            Created: "Created",
            Active: "Active",
            CancelRequest: "Cancel request",
            Canceled: "Canceled",
            Service: "Service",
            Deleted: "Deleted",
        },
        OrderState: {
            Draft: "Draft",
            Paid: "Paid",
            Canceled: "Canceled",
            NotPaid: "Not paid",
            WaitingForPayment: "Waiting for payment",
        },
    },
    common: {
        beverages: "Beverages",
        menu: "Menu",
        duration: {
            hour: 'h',
            minute: 'min'
        }
    },
    buttons: {
        continue: "Continue",
        back: "Back",
        confirmAndContinue: "Confirm and continue",
        changeBooking: "Change booking",
        confirmAndPay: "Confirm and pay"
    },
    menu: {
        home: '家',
        onBoard: '在船上',
        itineraries: '行程表',
        more: '更多',
        weddings: '婚礼',
        corporateEvents: '企业活动',
        privateHire: '私人租用',
        gifts: '礼物',
        contactUs: '联络我们',
        allBookings: '所有预订',
        newBooking: '新订租',
        currentBookings: '目前的预订',
        orders: '餐饮订单',
        promoCodes: '[医]角,角',
        newTours: '旅游创作',
        logout: '注销登记',
        booking: "预约服务",
        logIn: "登入",
        toursManagement: "参观团",
        private: "用户"
    },
    footer: {
        covid19Update: 'COVID19更新',
        customerCharter: '客户约章',
        faqs: '常见问题',
        privacyPolicy: '私隐政策',
        cookiePolicy: 'Cookie政策',
        termsAndConditions: '条款及细则',
        contactUs: '联络我们',
        careers: '职业生涯',
        modernSlaveryStatement: '现代奴隶制声明'
    },
    home: {
        title: 'PRIME Bus Tours',
        subTitle: '豪华观光巴士之旅',
        morePhotos: '查看更多照片',
        header: '豪华巴士观光旅游',
        departurePoint: '出发地点',
        details: '详情'
    },
    booking: {
        booking: 'Booking',
        date: 'Date',
        guests: '各位嘉宾',
        guestsWithDisabilities: 'Guest with disabilities',
        header: 'Trip booking',
        return: 'Return',
        route: 'Route',
        totalPrice: 'Total price',
        serviceCharge: 'Service charge',
        vat: 'Dont TVA',
        totalBefore: 'Total before',
        continue: '继续',
        upgrade: 'Upgrade',
        upgradeNote: 'Upgrade your table',
        tableRequirements: '表要求',
        time: 'Time',
        giftCardNum: 'Gift card number',
        separateTable: '个别表格',
        shareTable: '共享表',
        seatChoice: 'Seat choice',
        upperDeck: 'Upper deck',
        lowerDeck: 'Lower deck',
        giftCertificate: 'Gift certificate',
        contactAdmin: 'Contact the administrator',
        promoCode: 'Promo code',
        tourTypes: "Tour types",
        apply: 'Apply',
        seatsChosen: 'Seats chosen',
        back: 'Back',
        name: 'Name',
        phone: 'Phone number',
        haveConflict: 'You have a conflict with orders',
        accept: 'Accept',
        updateBusMap: 'Update bus map',
        specialRequests: 'Special requests',
        isAllDay: 'Is all day',
        timeFrom: 'Time from',
        timeTo: 'Time to',
        blockBookingForDraft: 'Block public booking',
        bookingType: 'Booking type',
        departurePoint: 'Starting location',
        arrivalPoint: 'End location',
        otherRoute: 'Other',
        route: 'Itinerary',
        comment: 'Comment',
        stops: 'Stops',
        addStop: 'Add stop',
        removeStop: 'Remove stop',
        checkConflicts: 'Check conflicts',
        approveConflicts: 'Approve conflicts',
        next: 'Next',
        addMenu: 'Add menu',
        removeMenu: 'Remove menu',
        addBeverage: 'Add beverage',
        removeBeverage: 'Remove beverage',
        tourPrice: 'Tour price',
        menusAndGuestsError: 'Menus amount not equals guests count',
        email: 'email',
        conflicts: { 
            title: 'Conflicts with existing bookings',
            tourType: 'Tour type',
            conflictType: 'Conflict type',
            place: 'Place',
            orderNumber: 'Order number',
            seatsNumbers: 'Seats numbers',
            isBlocking: 'Is blocking',
            departure: 'Departure',
            arrival: 'Arrival'
        },
        cameAll: 'Сame all',
        widgetError: {
            message: "要预订六人及以上的座位，请与管理员联系",
            link: "这里",
        },
    },
    bookingSummary: {
        header: 'Booking summary',
        headerPrivateHire: 'Private hire Summary',
        city: 'City',
        date: 'Date',
        itinerary: 'Itinerary',
        departureTime: 'Departure time',
        guests: 'Guests',
        disabledGuests: 'Guest with disabilities',
        table: 'Table',
        seats: 'Seats',
        tourPrice: 'Tour price',
        extras: 'Extras',
        promoCode: 'Promo code',
        certificate: 'Gift certificate',
        total: 'Total',
        vat: 'Including VAT',
        number: 'Booking number',
        name: 'Name',
        phoneNumber: 'Phone number',
        otherRoute: 'Other',
        menus: 'Menus',
        beverages: 'Beverages',
        extras: 'Extras',
        balance: 'balance*',
        number: 'Number'
    },
    menuBooking: {
        menusSelection: 'Menus selection',
        extrasSelection: 'Extras selection',
        included: 'Included',
        preferences: 'Choose your preferences',
        noExtraCharge: '(no extra charge)',
        allergy:'Allergy',
        seat: 'Seat',
        drinksSelection: 'Beverage selection',
        extraChargesApply: '(extra charges apply)', 
        noDrinksIncluded: 'Drinks aren’t included in the price',
        softDrinks: 'Soft drinks',
        hotDrinks: 'Hot drinks',
        alcoDrinks: 'Alcoholic drinks',
        info:'All drinks can be ordered in the bus before or during the tour, except alcoholic beverages. Alcoholic beverages can be ordered only at booking the tour. Should you change your opinion when on board, we will fully refund your unconsumed beverages',
        extra: 'Extra',
        notIncluded: '(Not included)',
        advancedOrder: 'You must order 48 hours in advance',
        disServCharge: '*discretionary service charge of 12% will be added to your bill',
        back:'Back',
        confirm:'Confirm and Continue',
        none:"None"

    },
    paymentBooking: {
        paymentConfirmation: 'payment confirmation',
        cardholdersName: 'Cardholder\'s name*',
        cardNumber: 'Card number*',
        expiryDate: 'Expiry date (mm/yyyy)*',
        cardVerificationCode: 'Card verification code*',
        name: 'Name',
        email: 'Email',
        repeatemail: 'repeatemail',
        pay: 'pay',
        selectAPaymentMethod: 'select a payment method'
    },
    weekDays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    auth: {
        login: "Login",
        password: "Password",
        logout: "Logout",
        wrongLogin: "Wrong login",
        wrongPassword: "Wrong password",
        logoutConfirmation: "Are you sure you want to logout?"
    },
    admin: {
        adminControl: "Administrator control",
        userList: "Administrator list",
        userName: "Name (login)",
        password: "Password",
        retypePassword: "Re-type password",
        retypePasswordWarning: "That’s not the same password as the first one",
        role: "Role",
        add: "Add",
        edit: "Edit",
        delete: "Delete",
        deleteConfirm: "Delete an account?",
        update: "Update",
        cancel: "Cancel",
    },
    orderInformation: {
        title: "Order information",
        back: "back to today",
    },
    createTour: {
        title: "Create tour schedule",
        dateStart: "Date start",
        dateEnd: "Date end",
        chooseWeekdays: "choose weekdays",
        type: "Type",
        Regular: "Regular",
        Service: "Service",
        weekdayStart: "Start day of week",
        weekdayEnd: "End day of week",
        setPeriodTime: "Set time",
        price: "Price",
        seatPrice: "Seat price",
        vipPrice: "VIP price",
        discount: "Discount (menu and Beverages)",
        addMore: "Add more",
        addTime: "Add time",
        removeTime: "Remove time",
        addSchedule: "Add schedule",
        removeSchedule: "Remove schedule",
        hasBeverages: "Beverages",
        hasMenu: "Menu",
        create: "Create",
        crossPeriod: "Period is crossing",
        weekdaysOuOfPeriod: "Weekdays out of period",
        wrongDatesRange: "Wrong range",
        wrongWeekdaysRange: "Wrong weekday range",
        serviceStart: 'Time start',
        serviceEnd: 'Time end',
        menuInTicket: 'Included in ticket',
        menuExtra: 'Available in extras',
        menuInTicketColumn:'T',
        menuExtraColumn: 'E',
        duplicateTours: "Duplicate tours",
        blockingTours: "Blocking tours",
        success: 'Successfully created tours',
    },
    tours: {
        management: {
            title: "Tours",
            tourTypes: "Tour types",
            filterButton: "Filter",
            filterCity: "City",
            filterDateFrom: "Date from",
            filterDateTo: "Date to",
            tours: 'Tours',
            deleteButton: 'Delete'
        },
        grid: {
            id: 'Id',
            datetime: 'Date/Time',
            type: 'Type',
            itinerary: 'Itinerary',
            duration: 'Duration'
        },
        hasPaidNotCancelledOrders: "The tour has paid orders. Cancel them first",
        tourSuccessfullyCancelled: "The tour has been successfully cancelled",
        tourCancellationError: 'Error cancelling the tour'
    },
    addCertificate: {
        title: 'Gift certificate',
        amountVariantId: 'Cost',
        hasSurprises: 'Surprise',
        extras: 'Extras',
        notRefundable: 'The cost of the gift certificate is non-refundable',
        howItWorks: 'Choose a gift that will surpass even the highest expectations: <br/> The opportunity to discover a memorable experience with PRIME Bus Tours',
        commentTitle: 'Comments',
        continue: 'Continue',
        vaidUntil: 'Valid until',
        howItWorks: 'There will be a text describing how the gift certificate works',
        confirm: 'Confirm'
    },
    giftCertificate: {
        number: 'Gift certificate',
        dateStart: 'Start date',
        dateEnd: 'Valid until',
        amount: 'Amount',
        comment: 'Comments',
        surprises: 'Surprises',
        total: 'Total',
        print: 'Print',
        emailSend: 'The certificate will be sent to your mail',
        customerService: 'Our customer care service',
        includingVat: 'Including VAT 20%',
        send: 'Send',
        management: {
            title: 'Gift certificates',
            statusFilter: 'Status',
            filterButton: 'Filter',
            total: 'Total',
            filterCity: 'City'
        },
        grid: {
            id: 'Id',
            number: 'Number',
            purchaseDate: 'Purchase date',
            dateStart: 'Start date',
            dateEnd: 'Expiry date',
            status: 'Status',
            amount: 'Amount',
            redeemedDate: 'Date & time of use',
            bookingNumber: 'Booking number',
            balance: 'Balance',
            count: 'Count of certificates'
        }
    },
    onBoard: {
        title: "On board",
        bus: "Bus",
        menu: "Menu",
        guide: "Guide",
        langs: {
            en: "English",
            de: "German",
            zh: "Chinese",
            ru: "Russian",
            fr: "French"
        },
        guideText: "On board our bus you can listen to the excursion program in several languages",
    },
    payment: {
        cardholdersName: 'Cardholder’s name',
        cardNumber: 'Card number',
        confirmation: {
            title: 'Payment confirmation',
            selectMethod: 'Select a payment method',
            payThis: 'pay this',
            expiryDate: 'Expiry date (mm/yyyy)',
            securityCode: 'Security code',
            bookingName: 'Booking name',
            email: 'Email',
            repeatEmail: 'Repeat email',
            phone: 'Telephone',
            agreePersonalData: 'I agree to the processing of personal data',
            iAgreeTo: 'I agree to',
            termsOfService: 'the Terms of Service',
            agreeNotifications: 'I agree to receive notifications and mailings',
            pay: 'Pay',
            incorrectCardNumber: 'Incorrect card number',
            amountToPay: 'Amount to pay'
        }
    },
    weekDaysNames: {
        'Sun': 'Sunday', 
        'Mon': 'Monday', 
        'Tue': 'Tuesday', 
        'Wed': 'Wednesday', 
        'Thu': 'Thursday', 
        'Fri': 'Friday', 
        'Sat': 'Saturday'
    },
    validation: {
        isRequired: "Field is required",
        isRequiredCheck: "Please, check the option",
        emailNotMatch: "Emails don't match",
        incorrectEmail: "Incorrect email format"
    },
    itineraries: {
        title: 'Itineraries',
        subTitle: 'London',
        tab1:'Itinerary 1',
        tab2:'Itinerary 2',
        text: 'In a nutshell the unrivalled combination between beauty and leisure on board this luxury bus-restaurant will provide an extraordinary experience ready to awaken your senses. Simply put, passengers on board will discover the most beautiful views of London while enjoying the best of its gastronomy',
        duration: 'Duration',
        duration1: '3 hours',
        languages: 'Languages',
        languages1: 'English, Spanish, French, Russian, Chinese, Arab, German',
        departurePoint: 'Departure point',
        departurePoint1: 'Stratton Street, London, W1J 8LT, United Kingdom',
        departureTime: 'Departure time',
        departureTime1: "Please be aware that the tour starts strictly at the time shown in your reservation confirmation. Unfortunately due to regulatory restriction we can't wait for late passengers. Please allow youself 10 min before the schedule tour time to come to departure point for ontime boarding",
        location: 'How to get',
        location1: "Please be aware that the tour starts strictly at the time shown in your reservation confirmation. Unfortunately due to regulatory restriction we can't wait for late passengers"
    },
    staticPages: {
        cookiePolicy: {
            header: "Cookie policy",
            whatAreCookies: {
                header: "What are cookies",
                paragraphs: [
                    "Cookies make the interaction between users and web sites faster and easier. Without cookies, it would be very difficult for a web site to allow a visitor to fill up a shopping cart or to remember the user's preferences or registration details for a future visit.",
                    "Web sites use cookies mainly because they save time and make the browsing experience more efficient and enjoyable. Web sites often use cookies for the purposes of collecting demographic information about their users.",
                    "Cookies enable web sites to monitor their users' web surfing habits and profile them for marketing purposes (for example, to find out which products or services they are interested in and send them targeted advertisements.",
                ], 
            },
            cookiesUsing: {
                header: "What cookies we are using",
                paragraph: "PRIME Bus Tours website using mainly below type of cookies.",
                cookiesType: [
                    "Strictly Necessary cookies",
                    "Performance cookies",
                    "Marketing cookies",
                    "Third Party cookies",
                ],
            },
        },
        contactUs: {
            subject: "Subject",
            text: "Text",
            phoneNumber: "Phone number",
        },
    },
    tourFB: {
        header: "Tour f&b",
        city: "City",
        dateFrom: "Date from",
        departureTime: "Departure time",
        tourEnd: "Tour end",
        tourType: "Tour type",
        guestsNumber: "Guests number",
        itinerary: "Itinerary",
        status: "Status",
        duration: "Duration",
        menu: "Menu",
        total: "TOTAL",
        beverage: "Beverage",
        softDrinks: "Soft drinks",
        hotDrinks: "Hot drinks",
        alcoDrinks: "Alcoholic drinks",
        extras: "Extras",
        specialRequests: "SPECIAL REQUESTS",
        comments: "COMMENTS",
        regularType: "Regular",
        privateType: "Private",
        number: 'Number'
    },
    currentBookings: {
        title: "Search",
        city: "City",
        dateFrom: "Date from",
        dateTo: "Date to",
        tourTypes: "Tour types",
        regular: "Regular",
        private: "Private",
        service: "Service",
        conflict: "Conflict",
        group: "Group",
        filter: "Filter",
        currentBookings: "Current bookings",
        dateTime: "Date/Time",
        type: "Type",
        status: "Status",
        itinerary: "Itinerary",
        duration: "Duration",
        conflicts: "Conflicts",
        guest: "Guests",
        extras: "Extras",
        of: " of ",
        paymentInformation: "Payment information",
        paid: "paid ",
        isWaiting: " is waiting",
        select: "Select",
        number: "Number"
    },
    order: {
        view: {
            header: "you have successfully paid for the order",
            summaryTitle: "your order summary",
            print: "print",
            emailText: "the order summary will be sent to your mail",
            serviceEmail: "our customer care service",
        },
    },
}