<template>
  <!-- 捡到卡片类型选择 -->
  <t-dialog
    v-model:visible="props.foundDialogVisible"
    close-on-overlay-click
    title="捡到卡片的类型"
    :cancel-btn="cancelBtn"
    :confirm-btn="confirmBtn"
    @confirm="onConfirm"
    @cancel="onCancel"
  >
    <div>
      <t-radio-group v-model="foundForm.cardType" class="radio-group-demo">
        <t-radio value="校园卡" label="校园卡" />
        <t-radio value="身份证" label="身份证" />
        <t-radio value="学生证" label="学生证" />
        <t-radio value="银行卡" label="银行卡" />
      </t-radio-group>
      <div v-if="showTypeError" class="error-message">请选择卡片类型</div>
    </div>
  </t-dialog>

  <!-- 卡片信息输入 -->
  <t-dialog
    v-model:visible="cardFoundVisible"
    close-on-overlay-click
    title="捡到卡片的信息"
    @cancel="foundCancel"
    @close="foundClose"
    :style="{ '--td-spacer-3': '20px' }"
  >
    <t-form ref="form" :data="foundForm" class="strict-vertical-form">
      <text class="textarea-example__label">{{ foundForm.cardType }}号</text>
      <t-input v-model="foundForm.number" class="strict-input" />
      <text class="textarea-example__label">留言</text>
      <t-textarea
        v-model="foundForm.message"
        class="textarea textarea-example"
        :placeholder="text"
        bordered
        indicator
      />
    </t-form>
    <div v-if="showTypeError1" class="error-message">请填写完整信息</div>
    <div class="footer">
      <t-button theme="default" size="large" variant="base" @click="foundCancel">取消</t-button>
      <t-button theme="primary" size="large" @click="foundConfirm">确认</t-button>
    </div>
  </t-dialog>

  <!-- 提交卡片信息成功 -->
  <t-dialog
    v-model:visible="successDialog"
    title="提交成功"
    confirm-btn="确认"
    :closeOnOverlayClick="true"
  >
    <div class="content-container">
      {{ content }}
    </div>
  </t-dialog>
</template>

<script setup lang="ts">
import { ref } from "vue";
import * as CryptoJS from 'crypto-js';
import { cardAPI } from "@/api/card";

const props = defineProps(["foundDialogVisible"]);
const emit = defineEmits<{
  (e: "update:foundDialogVisible", value: boolean): void;
}>();

const foundForm = ref({
  cardType: "",
  number: "",
  message: "",
});
const showTypeError = ref(false);
const showTypeError1 = ref(false);

//卡片类型选择窗口
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
const onConfirm = () => {
  console.log("cardType", foundForm.value);
  if (foundForm.value.cardType === "") {
    showTypeError.value = true; // 显示错误提示
    setTimeout(() => {
      showTypeError.value = false;
    }, 2500);
    return;
  }
  emit("update:foundDialogVisible", false); // 关闭弹窗
  cardFoundVisible.value = true;
};
const onCancel = () => {
  console.log("dialog:cancel");
  emit("update:foundDialogVisible", false); // 关闭弹窗
  foundForm.value.cardType = "";
};

//捡到卡片信息输入窗口
const text =
  "我捡到了你的卡片，请联系QQ1234567890。 \n我捡到了你的卡片，放在了XXX地点，请有空去拿。";
const cardFoundVisible = ref(false);

const foundConfirm = async () => {
  console.log("foundForm", foundForm.value);
  if (foundForm.value.message === "" || foundForm.value.number === "") {
    showTypeError1.value = true;
    setTimeout(() => {
      showTypeError1.value = false;
    }, 2500);
    return;
  }

  const salt = await getHashSalt();
  if (salt) {
    const hash = await calcHash(foundForm.value.number, foundForm.value.cardType, salt)
    await checkCards(hash)
  }

  afterConfirm(present.value)
};
const foundCancel = () => {
  cardFoundVisible.value = false;
  foundForm.value = {
    cardType: "",
    number: "",
    message: "",
  };
};
const foundClose = () => {
  foundForm.value = {
    cardType: "",
    number: "",
    message: "",
  };
};

// 提交成功弹窗
const successDialog = ref(false);
const content = ref('')
const present =ref(false)
function checkNumber(success:any) {
  if (success) {
    content.value = "我们已经找到了丢失卡片的同学，并发生了通知，感谢你的善举";
  } else {
    content.value = "我们正在寻找丢失卡片的同学，感谢你的善举";
  }
}

function afterConfirm(x:boolean) {
  cardFoundVisible.value = false;
  foundForm.value = {
    cardType: "",
    number: "",
    message: "",
  };
  checkNumber(x);
  successDialog.value = true;
}

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

//提交卡片信息
async function checkCards(hash:any) {
  try {
    const res = await cardAPI.checkcard({
      message: foundForm.value.message,
      cardType: foundForm.value.cardType,
      hash
    })
    console.log(res)
    if(res.message === "卡片不存在") present.value = false
    else present.value = true
  } catch (error) {
    console.error('提交卡片信息失败:', error)
  }
}
</script>

<style lang="less" scoped>
.radio-group-demo {
  background: #ffffff;
}
.error-message {
  color: red;
  margin-top: 8px;
  font-size: 14px;
}
.textarea-example {
  padding: 16px 16px 24px;
  background-color: var(--bg-color-demo, #fff);
}

.textarea {
  padding-top: 12px;
  padding-bottom: 12px;
  height: 150px;
  width: 100%;
}
.textarea-example__label {
  display: block;
  color: var(--td-text-color-primary, rgba(0, 0, 0, 0.9));
  font-size: 16px;
  line-height: 20px;
  padding-bottom: 5px;
  padding-top: 10px;
  padding-left: 16px;
  text-align: left;
}
.strict-input {
  border: 1px solid #dcdcdc;
  border-radius: 3px;
  padding: 4px 8px;
}
.footer {
  display: flex;
  justify-content: space-between;
  padding-right: 50px;
  padding-left: 50px;
  padding-top: 20px;
}
</style>
