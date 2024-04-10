<template>
  <v-row class="py-3">
    <v-col cols="6">
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Name</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.name }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Version</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.currentVersion }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Type</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.type }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Modified By</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">
            <span v-if="docInfo.modifiedBy">{{ docInfo.modifiedBy }}</span>
            <span class="text-muted fst-italic" v-else>Deleted Account</span>
          </div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Modified Time</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ dayjs(docInfo.modifiedDate).format('DD MMM YYYY, hh:mm A') }}</div>
        </v-col>
      </v-row>
    </v-col>
    <v-col cols="6">
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Categories</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">
            <el-tag class="me-1 mb-1" effect="light" v-for="category in docInfo.category.split(', ')">{{ category }}</el-tag>
          </div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        
          <v-col class="pt-0 pb-1" cols="12" md="3" lg="3">
            <v-label class="text-caption fw-bold">
              Description
              <v-btn class="ms-1" icon="mdi-pencil" size="x-small" density="comfortable" variant="text" @click="isEdit = true" v-if="!isEdit"/>
            </v-label>
          </v-col>
          <v-col class="pt-0 pb-1" cols="12" md="9" lg="9">
            <div class="text-body-2" v-if="!isEdit">{{ docInfo.description ?? '-' }}</div>
            <form class="d-flex" @submit.prevent="editDescription" v-else>
              <v-textarea
                density="compact"
                variant="outlined"
                v-model="newDocDescription"
                auto-grow
                hide-details
              />
              <div class="ps-1 d-flex flex-column">
                <v-btn icon="mdi-check" size="x-small" variant="text" type="submit"/>
                <v-btn icon="mdi-close" size="x-small" variant="text" @click="cancelEditDescription"/>
              </div>
            </form>
          </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup>
import dayjs from 'dayjs'

// Properties, Emit & Model
const props = defineProps({
  docInfo: Object
})

// Data
const isEdit = ref(false)
const newDocDescription = ref(props.docInfo.description)

// Methods
const cancelEditDescription = () => {
  isEdit.value = false
  newDocDescription.value = props.docInfo.description
}
const editDescription = async() => {
  try {
    const result = await useFetchCustom.$put(`/Repository/Description/${props.docInfo.id}`, {
      description: newDocDescription.value == '' ? null : newDocDescription.value
    })
    if (!result.error) {
      isEdit.value = false
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
}
</script>