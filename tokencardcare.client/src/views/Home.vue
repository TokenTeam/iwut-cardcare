<template>
  <t-navbar title="Card Care+" :fixed="false" />

  <p class="text1">你好，{{ userName }}</p>
  <p class="text2">Card Care+已经守护你{{ days }}天</p>

  <h3 class="cardTitle">校园卡</h3>
  <div class="schoolCard">
    <p
      style="
        color: white;
        padding-top: 15px;
        padding-left: 15px;
        font-size: 18px;
        font-weight: bold;
      "
    >
      武汉理工大学
    </p>
    <p class="bottom-text" style="color: white">卡号：{{ cardId }}</p>
    <img class="logo-image" src="@/assets/img/logo.png" />
  </div>

  <h3 v-show="foundCards.length" class="cardTitle">已捡到的卡片</h3>
  <multi-cards
    v-show="foundCards.length"
    :cards="foundCards"
    :add="false"
    :lost="true"
    :cardId="cardId"
    @update:clickCards="(card: any) => {showGotDialog(card)}"
    @update:deleteCard="handleConfirm"
  />

  <h3 class="cardTitle">其他卡片</h3>
  <multi-cards
    :cards="otherCards"
    @update:cards="() => (popupVisible = true)"
    :cardId="cardId"
    :add="true"
    :lost="false"
    @update:deleteCard="handleConfirm"
  />

  <t-popup v-model="popupVisible" placement="bottom" style="height: 258px">
    <t-picker
      v-model="pickedItem"
      title="选择卡片类型"
      :columns="cardOptions"
      @confirm="onConfirm"
      @cancel="
        popupVisible = false;
        pickedItem = [];
      "
      @pick="onPick"
    >
    </t-picker>
  </t-popup>

  <form-modal
    v-model:isShowDialog="formVisible"
    v-model:title="formTitle"
    :cardId="cardId"
    @confirm="handleConfirm"
    @cancel="handleCancel"
  />

  <!-- FabMenu组件 -->
  <fab-menu @alert-click="handleAlert" @help-click="handleHelp" />

  <!--丢失卡片对话框-->
  <t-dialog
    v-model:visible="lostDialog"
    title="丢失卡片"
    confirm-btn="确认"
    :closeOnOverlayClick="true"
  >
    <div class="content-container">
      {{ content }}
    </div>
  </t-dialog>

  <!--捡到卡片对话框-->
  <foundDialog v-model:foundDialogVisible="foundDialogVisible" />

  <!--卡片被捡到话框-->
  <t-dialog
    v-model:visible="gotDialog"
    title="卡片信息"
    confirm-btn="确认"
    :closeOnOverlayClick="true"
  >
    <div class="content-container">
      <br />
      你的{{ gotCardType }}被好心人捡到了！
      <br />
      <br />
      对方留言： {{ content1 }}
    </div>
  </t-dialog>

  <!-- 登录提醒窗口-->
  <t-dialog
    v-model:visible="isLoginDialog"
    title="登录提醒"
    confirm-btn="确认"
    :closeOnOverlayClick="true"
  >
    <div class="content-container">
      <br />
      <br />
      请先在掌上吾理进行用户登录，再使用本功能。
    </div>
  </t-dialog>
</template>

<script setup lang="ts">
import { h } from "vue";
import { AddIcon } from "tdesign-icons-vue-next";
import type { PickerValue } from "tdesign-mobile-vue";
import { ref, reactive } from "vue";
import MultiCards from "@/components/MultiCards.vue";
import FormModal from "@/components/FormModal.vue";
import type { RefSymbol } from "@vue/reactivity";
import FabMenu from "@/components/FabMenu/index.vue";
import FoundDialog from "@/components/FoundDialog.vue";
import { getAppInfo, getStudentProfile } from "@/api/rpc";
import { onMounted } from "vue";
import { cardAPI } from "@/api/card";
import { messageConfig } from "element-plus";

const isLoginDialog = ref(false);
const userName = ref("");
const cardId = ref("");
const cardList = ref<any>([]);
const foundCardList = ref<any>([]);
let days = ref(365);

onMounted(async () => {
  getAppInfo().then((res) => console.log(res));
  await getStudentProfile().then((res) => {
    console.log(res);
    if (!res.hasProfile) {
      isLoginDialog.value = true;
      return;
    }
    userName.value = res.name;
    cardId.value = res.cardId;
  });

  await getCardList();
  updateCardList();
});

//卡片类型列表
async function test() {
  try {
    const res = await cardAPI.cardtypes();
    console.log(res);
  } catch (error) {
    console.error("获取信息失败:", error);
  }
}
//获取卡片列表
async function getCardList() {
  try {
    console.log(cardId.value);
    const res = await cardAPI.getcards({
      studentNumber: cardId.value,
    });
    console.log(res);
    cardList.value = res.data.filter((item: any) => item.cardType !== "校园卡");
    foundCardList.value = res.data.filter((item: any) => item.state === 1);
    //state:0普通状态，1被捡到
  } catch (error) {
    console.error("获取卡片列表信息失败:", error);
  }
}

//卡片信息
const otherCards = ref([]);
const foundCards = ref([]);
//更新卡片显示界面
function updateCardList() {
  //已捡到的卡片信息
  foundCards.value = foundCardList.value.map((item: any, index: number) => ({
    id: index + 1,
    type: item.cardType,
    name: item.cardName,
    number: item.hash,
    message: item.message
  }));
  console.log("已捡到", foundCards.value);
  //其他卡片的信息
  const otherCardsList = cardList.value.filter((item: any) => item.state === 0);
  otherCards.value = otherCardsList.map((item: any, index: number) => ({
    id: index + 1,
    type: item.cardType,
    name: item.cardName,
    number: item.hash,
    message: item.message
  }));
  console.log("其他卡片", otherCards.value);
}

// FabMenu组件
const handleAlert = () => {
  lostDialog.value = true;
};
const handleHelp = () => {
  foundDialogVisible.value = true;
};

//底部弹窗
let popupVisible = ref(false);
const cardOptions = () => {
  return [
    {
      label: "身份证",
      value: "身份证",
    },
    {
      label: "学生证",
      value: "学生证",
    },
    {
      label: "银行卡",
      value: "银行卡",
    },
  ];
};
const pickedItem = ref([]);
const formVisible = ref(false);
const onConfirm = (val: string[], context: number[]) => {
  console.log(val);
  //console.log('context', context);
  console.log(pickedItem.value[0]);
  formTitle.value = pickedItem.value[0];
  popupVisible.value = false;
  formVisible.value = true;
};
const onPick = (value: PickerValue[], context: any) => {
  console.log("value", value);
  //console.log('context', context);
};

//formModals设置
const formTitle = ref("");
//添加卡片
const handleConfirm = async () => {
  await getCardList();
  updateCardList();
};
const handleCancel = () => {
  console.log("父组件收到取消");
};

//丢失卡片对话框
const lostDialog = ref(false);
const content =
  "\n如果您已经添加了该卡片，请静候佳音。 \n如果丢失卡片还没有添加，请手动添加该卡片信息，我们会尝试为你寻找。";

//捡到卡片对话框
const foundDialogVisible = ref(false);

//卡片被捡到话框
const gotDialog = ref(false);
const gotCardType = ref("卡片");
const content1 = ref('')
if (foundCards.value.length) gotDialog.value = true;
function showGotDialog(card: any) {
  gotCardType.value = card.type;
  content1.value = card.message
  gotDialog.value = true;
}
</script>

<style scoped>
.t-navbar {
  margin-bottom: 16px;
}
.text1 {
  padding-top: 10px;
}
.text1,
.text2 {
  font-size: 14px;
  line-height: 10px;
  margin-bottom: 16px;
  display: flex;
  justify-content: flex-end;
  padding-right: 20px;
}
.cardTitle {
  padding-top: 20px;
  padding-left: 20px;
}
.schoolCard {
  background-color: #369768;
  height: 120px;
  width: 300px;
  margin: 0 auto;
  border-radius: 10px;
  position: relative;
  box-shadow: 5px 6px 12px rgba(0, 0, 0, 0.3);
}
.bottom-text {
  position: absolute;
  bottom: 0;
  left: 50%;
  width: 100%;
  margin: 0;
  padding: 10px;
}

/* 强制穿透所有层级 */
:deep(.t-fab) {
  position: fixed !important;
  bottom: 80px;
  right: 24px;
  transform: translateZ(100px);
}
/* 关键：限制卡片堆叠的 z-index 范围 */
:deep(.bank-card) {
  z-index: auto !important; /* 禁用动态 z-index */
}

.popup-demo {
  padding: 0 16px;
}

.header {
  display: flex;
  align-items: center;
  height: 116rpx;
}

.title {
  flex: 1;
  text-align: center;
  font-weight: 600;
  font-size: 18px;
  color: var(--td-text-color-primary, rgba(0, 0, 0, 0.9));
}

.btn {
  font-size: 16px;
  padding: 16px;
}

.btn--cancel {
  color: var(--td-text-color-secondary, rgba(0, 0, 0, 0.6));
}

.btn--confirm {
  color: #0052d9;
}

.bottom-text {
  font-size: 16px;
  letter-spacing: 1px;
  position: absolute;
  bottom: 0px;
  left: 5px;
  font-family: "Courier New", monospace;
}

.content-container {
  height: 150px;
  overflow-y: auto;
  font-size: 16px;
  color: var(--td-font-gray-2);
  line-height: 24px;
  white-space: pre-line;
  text-align: left;

  &::-webkit-scrollbar {
    display: none;
    width: 0;
    height: 0;
  }
}

.logo-image {
  position: absolute;
  bottom: 5px;
  right: 10px;
}
</style>
