const baseUrl = '/api/UserProfile';

export const getAllUsers = () => {
    return fetch(`${baseUrl}`).then((res) => res.json());
};

export const getUser = (id) => {
    return fetch(`${baseUrl}/getWithVideosAndComments${id}`).then((res) => res.json());
};