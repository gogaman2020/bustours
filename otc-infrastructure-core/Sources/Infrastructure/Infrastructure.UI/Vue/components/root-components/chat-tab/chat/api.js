import axios from 'axios'

export const chatMicroserviceUrl = process.env.VUE_APP_CHAT_API_URL;

export const getPromiseGet = (options) => {
    return new Promise((resolve, reject) => {
      axios.get(options.url, {withCredentials: options.withCredentials})
          .then(response => {
            resolve(response.data);
          })
          .catch(error => {
            reject(error);
          });
    });
  };

export const getPromiseHeaderGet = (options) => {
    return new Promise((resolve, reject) => {
        axios.get(options.url, { withCredentials: options.withCredentials, headers: {
                'Accept': 'application/json, text/javascript, */*; q=0.01',
                'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
            }})
            .then(response => {
                resolve(response.data);
            })
            .catch(error => {
                reject(error);
            });
    });
};

export const getPromisePost = (options) => {
    return new Promise((resolve, reject) => {
        axios.post(options.url, options.params, {withCredentials: options.withCredentials})
            .then(response => {
                resolve(response.data);
            })
            .catch(error => {
                reject(error);
            });
    });
};

export const getPromiseHeaderPost = (options) => {
  return new Promise((resolve, reject) => {
      axios.post(options.url, options.params, {withCredentials: options.withCredentials, headers: {
        'Accept': 'application/json, text/javascript, */*; q=0.01',
        'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'}
      })
          .then(response => {
              resolve(response.data);
          })
          .catch(error => {
              reject(error);
          });
  });
};

export const getCurrentChats = (params) => {
    const options = {
      url: `${chatMicroserviceUrl}/api/chat/current-chats`,
      withCredentials: true,
      params: params
    };
  
    return getPromisePost(options);
};

export const getAllMessages = (params) => {

  const options = {
    url: `${chatMicroserviceUrl}/chat/GetNewChatMessagesWithStringResult`,
    withCredentials: true,
    params: params
  };

  return getPromiseHeaderPost(options);
};

export const sendMessage = (params) => {
  const options = {
    url: `${chatMicroserviceUrl}/chat/AddChatMessageWithStringResult`, 
    withCredentials: true,
    params: params
  };

  return getPromiseHeaderPost(options);
};

export const setChatMessageReaded = (param) => {
    const options = {
        url: `${chatMicroserviceUrl}/api/chat/chat-messages-readed?id=${param}`,
        withCredentials: true
    };

    return getPromiseHeaderGet(options);
};