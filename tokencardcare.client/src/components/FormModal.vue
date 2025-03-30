<template>
  <t-dialog
    v-model:visible="props.isShowDialog"
    close-on-overlay-click
    :title="`添加${title}`"
    :cancel-btn="cancelBtn"
    :confirm-btn="confirmBtn"
    @confirm="onConfirm"
    @cancel="onCancel"
    style="width: 350px"
    :style="{ '--td-dialog-width': 'auto' }"
  >
    <t-form ref="form" :data="formData" class="strict-vertical-form">
      <t-form-item
        :label="`${title}号`"
        name="cardNumber"
        class="strict-form-item"
      >
        <t-input v-model="formData.number" class="strict-input" />
      </t-form-item>
      <span class="security-tip"
        >*请确认添加的是你自己的卡片，如果捡到别人的卡片，请点击右下角加号，选择“我捡到卡片”</span
      >
      <t-form-item label="卡片名称" name="cardName" class="strict-form-item">
        <t-input v-model="formData.name" class="strict-input" />
      </t-form-item>
    </t-form>
  </t-dialog>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { cardAPI } from "@/api/card";
import * as CryptoJS from 'crypto-js';

const props = defineProps(["isShowDialog", "title", "cardId"]);

const emit = defineEmits<{
  (e: "update:isShowDialog", value: boolean): void;
  (e: "confirm"): void;
  (e: "cancel"): void;
}>();

const confirmBtn = {
  content: "确认",
  variant: "text",
  size: "large",
};
const cancelBtn = {
  content: "取消",
  variant: "text",
  size: "large",
};

const formData = ref({
  number: "",
  name: "",
});


// 确认事件
const onConfirm = async () => {
  console.log("dialog:confirm", formData.value);
  const salt = await getHashSalt();
  if (salt) {
    console.log(props.cardId)
    const hash = await calcHash(formData.value.number, props.title, salt);
    await addCard(hash);
    emit("confirm"); // 触发父组件事件
    emit("update:isShowDialog", false); // 关闭弹窗
    formData.value = { number: "", name: "" };
  }
};

// 取消事件
const onCancel = () => {
  console.log("dialog:cancel");
  formData.value = { number: "", name: "" };
  emit("cancel"); // 触发父组件事件
  emit("update:isShowDialog", false); // 关闭弹窗
};

//获取HashSalt
async function getHashSalt() {
  try {
    const res = await cardAPI.getSalt()
    console.log(res)
    return res
  } catch (error) {
    console.error('获取HashSalt失败:', error)
  }
}

async function calcHash(id: string, cardType: string, salt: any): Promise<string> {
    const str = `${id}${cardType}${salt}`;
    // 使用 crypto-js 计算 SHA-256 哈希
    const hash = CryptoJS.SHA256(str);
    // 将哈希值转换为 Base64 字符串
    const hashBase64 = CryptoJS.enc.Base64.stringify(hash);
    return hashBase64;
}

//添加卡片
async function addCard(hash: string) {
  try {
    const res = await cardAPI.newcard({
      studentNumber: props.cardId,
      cardType: props.title,
      hash,
      cardName: formData.value.name,
    });
    console.log(res);
  } catch (error) {
    console.error("添加卡片失败:", error);
  }
}
</script>

<style scoped>
/* 强制竖直布局的核心代码 */
.strict-vertical-form :deep(.t-form__item) {
  display: block !important;
  width: 100%;
}

/* 深度穿透修改 TDesign 组件样式 */
.strict-form-item:deep(.t-form-item) {
  display: block !important;
  width: 100%;
}

.strict-form-item:deep(.t-form-item__label) {
  display: block !important;
  width: 100% !important;
  margin-bottom: 8px !important;
  text-align: left !important;
  /* 新增：解决可能存在的居中问题 */
}

.strict-form-item:deep(.t-form-item__content) {
  display: block !important;
  width: 100% !important;
  margin-left: 0 !important;
}

/* 确保输入框容器 100% 宽度 */
.strict-form-item:deep(.t-input) {
  width: 100% !important;
}

.security-tip {
  display: block;
  font-size: 12px;
  color: #e34d59;
  line-height: 1.5;
}
</style>
