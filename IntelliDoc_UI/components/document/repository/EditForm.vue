<template>
  <form @submit.prevent="editDoc">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col>
            <v-label class="text-caption">Document Name</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="editDocInfo.nameWithoutExt.value"
              :suffix="editDocInfo.extension"
              :error-messages="editDocInfo.nameWithoutExt.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-label class="text-caption">Categories</v-label>
            <v-select
              :items="docCategoryList"
              item-title="name"
              item-value="name"
              placeholder="File Type"
              density="compact"
              variant="outlined"
              v-model="editDocInfo.category"
              @update:modelValue="editDocInfo.categoryLength.value = editDocInfo.category.length"
              :error-messages="editDocInfo.categoryLength.errorMessage"
              multiple
              item-props
              hide-details="auto"
            >
              <template #selection="{ item, index }">
                <v-chip class="text-caption" v-if="index < 3">{{ item.title }}</v-chip>
                <span class="text-grey text-caption align-self-center" v-if="index == 3">
                  (+{{ editDocInfo.category.length - 3 }} others) 
                </span>
              </template>
            </v-select>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-label class="text-caption">Description</v-label>
            <v-textarea
              density="compact"
              variant="outlined"
              rows="5"
              v-model="editDocInfo.description"
              no-resize
              hide-details
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">
      <v-btn color="primary" type="submit" :disabled="loading">
        <v-progress-circular class="me-2" color="primary" :size="18" :width="3" indeterminate v-if="loading"/>
        {{ loading ? 'Submitting' : 'Submit' }}
      </v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'

// Properties, Emit & Model
const props = defineProps({
  doc: Object,
  docCategoryList: Array
})
const emit = defineEmits(['close-modal'])

// Data
const loading = ref(false)
const { handleSubmit } = useForm({
  initialValues: {
    nameWithoutExt: props.doc.name.slice(0, props.doc.name.lastIndexOf(".")),
    categoryLength: props.doc.category.split(', ').length,
  },
  validationSchema: {
    nameWithoutExt(value) {
      return value ? true : 'Document name is required'
    },
    categoryLength(value) {
      return value > 0 ? true : 'Category is required'
    }
  }
})
const editDocInfo = ref({
  nameWithoutExt: useField('nameWithoutExt'),
  extension: props.doc.name.slice(props.doc.name.lastIndexOf(".")),
  category: props.doc.category.split(', '),
  categoryLength: useField('categoryLength'),
  description: props.doc.description
})

// Methods
const editDoc = handleSubmit(async(values) => {
  loading.value = true
  try {
    const result = await useFetchCustom.$put(`/Repository/Edit/${props.doc.id}`, {
      name: `${values.nameWithoutExt}${editDocInfo.value.extension}`,
      category: editDocInfo.value.category.sort().join(', '),
      description: editDocInfo.value.description == '' ? null : editDocInfo.value.description
    })
    
    if (!result.error) {
      emit('close-modal', false)
      editDocInfo.value.nameWithoutExt.resetField()
      editDocInfo.value.extension = null
      editDocInfo.value.category = []
      editDocInfo.value.categoryLength.resetField()
      editDocInfo.value.description = null
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
  loading.value = false
})
</script>