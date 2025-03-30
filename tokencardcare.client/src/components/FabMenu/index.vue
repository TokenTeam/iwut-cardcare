<template>
    <!-- 遮罩层 -->
    <div 
      v-show="showMenu" 
      class="fab-mask"
      @click="closeMenu"
    ></div>
  
    <!-- 主按钮及子菜单 -->
    <teleport to="body">
        <t-fab 
          :icon="mainIcon" 
          @click="toggleMenu"
          teleport-to="body"
          class="main-fab"
        />
    </teleport>
    <teleport to="body">
        <transition-group 
          name="fab-expand"
          class="sub-fab-container"
          tag="div"
        >
          <t-fab
            v-show="showMenu"
            key="alert"
            class="sub-fab alert"
            :icon="alertIcon"
            text="我丢失了卡片"
            draggable="true"
            @click="handleAlert"  
            style="right: 0px; bottom: 70px;"
          />
          <t-fab
            v-show="showMenu"
            key="help"
            class="sub-fab"
            :icon="helpIcon"
            text="我捡到了卡片"
            draggable="true"
            @click="handleHelp" 
            style="right: 70px; bottom: 0px;"
          />
        </transition-group>
    </teleport>
</template>
  
<script setup lang="ts">
import { h } from 'vue';
import { ref } from 'vue';
import { AddIcon, HelpIcon, ErrorIcon } from 'tdesign-icons-vue-next';

const emit = defineEmits(['alert-click', 'help-click']);

const showMenu = ref(false);
const mainIcon = () => h(AddIcon, { size: '24px' });
const alertIcon = () => h(ErrorIcon, { size: '24px' });
const helpIcon = () => h(HelpIcon, { size: '24px' });

const toggleMenu = () => {
  showMenu.value = !showMenu.value;
};

const closeMenu = () => {
  showMenu.value = false;
};

const handleAlert = () => {
  emit('alert-click');
  closeMenu();
};

const handleHelp = () => {
  emit('help-click');
  closeMenu();
};
</script>
  
<style scoped>
/* 遮罩层样式 */
.fab-mask {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.55);
  z-index: 100; /* 保持低于按钮层级 */
}

/* 主按钮定位 */
.main-fab :deep(.t-fab) {
  position: fixed !important;
  bottom: 80px !important;
  right: 24px !important;
  z-index: 10000 !important; /* 提升至更高层级 */
  transition: transform 0.3s ease;
}

/* 主按钮定位 */
.main-fab :deep(.t-fab) {
  position: fixed !important;
  bottom: 80px !important;
  right: 24px !important;
  z-index: 10000 !important;
  transition: transform 0.3s ease;
}

/* 子按钮容器 */
.sub-fab-container {
  position: fixed;
  bottom: 50px;
  right: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  z-index: 10000;
  pointer-events: auto; 
  transform: translateY(-100%);
}

/* 子按钮基础定位 */
.sub-fab :deep(.t-fab) {
  position: absolute !important;
  pointer-events: auto !important; 
  background: #fff !important;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
  transition: all 0.3s cubic-bezier(0.18, 0.89, 0.32, 1.28);
}

/* 动画优化 */
.fab-expand-enter-active {
  transition: all 0.3s ease-out;
}
.fab-expand-leave-active {
  transition: all 0.2s ease-in;
}
.fab-expand-enter-from {
  opacity: 0;
  transform: translateY(40px) scale(0.3) !important;
}
.fab-expand-leave-to {
  opacity: 0;
  transform: translateY(-20px) scale(0.5) !important;
}
</style>