import axios, { AxiosError, AxiosResponse } from 'axios';

import { Activity } from '../../models/Activity.model';

import { toast } from "react-toastify";
import { redirectTo } from "../../utils/routing.utils";

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay)
    })
}

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(async response => {
    await sleep(1000);
    return response;
}, (error: AxiosError) => {
    let { data, status, config } = error.response!;
    const newData = data as any;

    switch (status) {
        case 400:
            if (typeof data === 'string') {
                return toast.error(data);
            }
            if (config.method === 'get' && newData.errors.hasOwnProperty('id')) {
                return redirectTo('not-found');
            }

            const { errors } = newData;
            if (errors) {
                const modalStateErrors = [];
                for (const key in errors) {
                    if (errors[key]) {
                        modalStateErrors.push(errors[key]);
                    }
                }
                throw modalStateErrors.flat();
            }
            break;
        case 404:
            redirectTo('not-found');
            break;
        case 500:
            toast.error('server error');
            break;
        default:
            toast.error('unknown error');
            break;
    }

    return Promise.reject(error);
});

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T> (url: string) => axios.get<T>(url).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
    put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody),
}

const Activities = {
    list: () => requests.get<Activity[]>('/activities'),
    details: (id: string) => requests.get<Activity>(`/activities/${id}`),
    create: (activity: Activity) => requests.post('/activities', activity),
    update: (activity: Activity) => requests.put(`/activities/${activity.id}`, activity),
    delete: (id: string) => requests.delete(`/activities/${id}`)
}

const agent = {
    Activities
}

export default agent;