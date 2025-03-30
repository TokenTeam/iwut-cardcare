<template>
  <div class="card-stack-container">
    <div class="card-stack">
      <!-- 卡片列表 -->
      <div
        v-for="(card, index) in cards"
        :key="card.id"
        class="bank-card"
        :style="{
          'z-index': cards.length - index,
          transform: `translateY(${index * 20}px)`,
          background: colorBar[index % 8],
          'margin-top': index === 0 ? '0' : '-100px',
          top: isExpanded === card.id ? '-10px' : '0',
        }"
        @click="toggleCard(card)"
      >
        <div class="card-content">
          <div class="card-type">{{ card.name }}</div>
          <icon v-if="!props.lost"
            name="delete"
            style="position: absolute; right: 10px; top: 10px"
            @click="deleteCard(card)"
          />
          <icon v-else
            name="refresh"
            style="position: absolute; right: 10px; top: 10px"
            @click="deleteCard(card)"
          />
        </div>
      </div>

      <!-- 添加卡片按钮 -->
      <div v-show="props.add" class="add-card" @click="addNewCard">
        <div class="add-icon">+</div>
        <div class="add-text">添加卡片</div>
      </div>
    </div>

    <t-dialog
      v-model:visible="deleteDialog"
      close-on-overlay-click
      :title="`${title}卡片`"
      :content="`确认${title}${cardName}吗`"
      cancel-btn="取消"
      :confirm-btn="{ content: '确定', theme: 'danger' }"
      @confirm="onConfirm"
      @close="onClose"
    >
    </t-dialog>
  </div>
</template>

<script setup lang="ts">
import { Icon } from "tdesign-icons-vue-next";
import { ref } from "vue";
import { cardAPI } from "@/api/card";

const props = defineProps(["cards", "add", "lost", "cardId"]);
const emit = defineEmits<{
  (e: "update:cards"): void;
  (e: "update:clickCards", card: any): void;
  (e: "update:deleteCard"): void;
}>();

const colorBar = [
  "#7D6CF6",
  "#5CA1F8",
  "#8FBAF9",
  "#7CDDC0",
  "#80E38F",
  "#ECC65E",
  "#ED935C",
  "#E66D69",
];

//点击动画功能
const isExpanded = ref(null);
const toggleCard = (card:any) => {
  emit("update:clickCards", card);
  if (card.id === props.cards.length) return;
  isExpanded.value = isExpanded.value === card.id ? null : card.id;
};

const addNewCard = () => {
  emit("update:cards");
};

//删除卡片
const deleteDialog = ref(false);
const cardName = ref("");
const title = ref("");

function deleteCard(card:any) {
  console.log(card);
  if (props.lost) {
    title.value = "重置";
  } else {
    title.value = "删除";
  }
  pickedCard.value = card;
  cardName.value = card.name;
  deleteDialog.value = true;
}

const onConfirm = async() => {
  console.log(pickedCard.value);
  if (props.lost) {
    // 重制卡片
    await resetCard();
  } else {
    // 删除卡片
    await removecard();
  }
  console.log("删除卡片成功");
  emit("update:deleteCard");
  console.log("已发送update:deleteCard事件");
};

const onClose = () => {
  console.log("dialog: close");
};

interface Card {
  id: number;
  name: string;
  number: string;
  type: string;
}
const pickedCard = ref<Card | Record<string, never>>({});
//删除卡片接口
async function removecard() {
  try {
    const res = await cardAPI.removecard({
      studentNumber: props.cardId,
      hash: pickedCard.value.number,
    });
    console.log(res);
  } catch (error) {
    console.error("删除卡片失败:", error);
  }
}
//重制卡片接口
async function resetCard() {
  try {
    const res = await cardAPI.resetcard({
      studentNumber: props.cardId,
      hash: pickedCard.value.number,
      cardType: pickedCard.value.type,
    });
    console.log(res);
  } catch (error) {
    console.error("重制卡片失败:", error);
  }
}
</script>

<style scoped>
.card-stack-container {
  width: 300px;
  max-width: 390px;
  margin: 0 auto;
  padding: 0px;
}

.card-stack {
  position: relative;
  min-height: 200px;
}

.bank-card {
  width: 100%;
  height: 120px;
  border-radius: 12px;
  padding: 15px;
  color: white;
  position: relative;
  box-shadow: 1px 3px 10px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  cursor: pointer;
  -webkit-tap-highlight-color: transparent;
  outline: none;
  user-select: none;
}

.card-type {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 15px;
}

.add-card {
  width: 100%;
  height: 100px;
  border: 2px dashed #ccc;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #666;
  cursor: pointer;
  background: rgba(255, 255, 255, 0.9);
  transition: all 0.3s;
  margin-top: 50px;
  margin-bottom: 30px;
}

.add-card:hover {
  border-color: #3498db;
  color: #3498db;
  background: rgba(52, 152, 219, 0.1);
}

.add-icon {
  font-size: 28px;
  line-height: 1;
}

.add-text {
  font-size: 16px;
  margin-top: 5px;
}
</style>
