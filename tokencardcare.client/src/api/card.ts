import { get, post } from "./base";

//定义接口类型
declare namespace cardAPI {
    interface getCardsParams {
        studentNumber: string;
    }
    interface newCardInfo {
        studentNumber: string;
        cardName: string;
        cardType: string;
        hash: string;
    }
    interface deleteCardInfo {
        studentNumber: string;
        hash: string;
    }
    interface resetcardInfo {
        studentNumber: string;
        hash: string;
        cardType: string
    }
    interface checkcardInfo {
        message: string;
        cardType: string;
        hash: string;
    }

    interface cardListItem {
        cardName: string;
        cardType: string;
        hash: string;
        state: string;
    }
}

//具体接口实现
export const cardAPI = {
    //获取卡片类型
    cardtypes() {
        return get('/cardcare/cardtypes')
    },
    //获取卡片列表
    getcards(data: cardAPI.getCardsParams) {
        return post<[cardAPI.cardListItem]>('/cardcare/getcards', data)
    },
    //获取HashSalt
    getSalt() {
        return get('/cardcare/salt')
    },
    //上传卡片信息
    newcard(data: cardAPI.newCardInfo) {
        return post<null>('/cardcare/newcard', data)
    },
    //删除卡片
    removecard(data: cardAPI.deleteCardInfo) {
        return post('/cardcare/removecard', data)
    },
    //重置卡片
    resetcard(data: cardAPI.resetcardInfo) {
        return post('/cardcare/resetcard', data)
    },
    //检查卡片是否存在
    checkcard(data: cardAPI.checkcardInfo) {
        return post('/cardcare/checkcard', data)
    }
}