<template>
  <v-row class="m-0">
    <v-col class="pt-0">
      <v-list-item-title class="pb-1 text-body-1 fw-bold">Relevant Documents</v-list-item-title>
      <el-scrollbar height="220px" always>
        <el-empty class="p-0" :image-size="130" description="No Relevant Documents" v-if="relatedDocs.length == 0"/>
        <v-list
          density="compact"
          :items="relatedDocs"
          item-props
          v-else
        >
          <template #item="{ props }">
            <v-list-item
              class="p-0 text-nowrap"
              :prepend-icon="'text-h5 '+ (props.type == 'PDF' ? 'mdi-file-pdf-box' : 'mdi-file-word-box')"
              style="min-height: 32px;"
            >
              <span class="row-link" @click="selectedDocInfo = props">
                <v-tooltip :text="props.name" activator="parent" location="top" offset="2"/>
                {{ props.name }}
                <v-menu activator="parent" offset="5">
                  <v-card class="text-body-2" width="450px">
                    <div class="px-2 py-3">
                      <DocumentRepositoryDescriptionInfo :doc-info="props" @open-edit-modal="emit('select-doc', ['Edit', props])"/>
                    </div>
                  </v-card>
                </v-menu>
              </span>
              <template #append>
                <v-menu width="220" location="left" offset="2">
                  <template #activator="{ props }">
                    <v-btn class="me-3" size="small" variant="text" density="comfortable" :="props" icon>
                      <v-icon icon="mdi-dots-vertical"/>
                      <v-tooltip text="More Actions" location="top" offset="2" activator="parent"/>
                    </v-btn>
                  </template>
                  <v-list class="text-body-1" density="compact" elevation="4">
                    <v-list-item prepend-icon="mdi-download-outline" @click="downloadDoc(props)">Download</v-list-item>
                    <v-list-item prepend-icon="mdi-upload-outline" @click="emit('select-doc', ['Update', props])">Update</v-list-item>
                    <v-list-item prepend-icon="mdi-pencil-outline" @click="emit('select-doc', ['Edit', props])">Edit</v-list-item>
                    <v-list-item prepend-icon="mdi-rename-outline" @click="emit('select-doc', ['Rename', props])">Rename</v-list-item>
                    <v-divider class="m-2"/>
                    <v-list-item prepend-icon="mdi-history" @click="emit('select-doc', ['VersionHistory', props])">Version History</v-list-item>
                    <v-divider class="m-2"/>
                    <v-list-item prepend-icon="mdi-archive-outline" @click="archiveDoc(props.id)">Archive</v-list-item>
                  </v-list>
                </v-menu>
              </template>
            </v-list-item>
          </template>
        </v-list>
      </el-scrollbar>
    </v-col>
  </v-row>
  <v-row class="m-0">
    <v-col class="py-0 d-flex justify-content-end">
      <v-btn
        color="primary"
        prepend-icon="mdi-paperclip-plus"
        size="small"
        variant="tonal"
        @click="emit('create-doc', mainDocId)"
      >
        Add Related Document
      </v-btn>
    </v-col>
  </v-row>
</template>

<script setup>
import { Buffer } from 'buffer'

// Properties, Emit & Model
const props = defineProps({
  mainDocId: Number,  
})
const emit = defineEmits(['select-doc', 'create-doc'])

// Data
const selectedDocInfo = ref(null)
const { data: relatedDocs, pending: loading } = await useFetchCustom.$get(`/Repository/RelatedDocs/${props.mainDocId}`)

// Methods
const downloadDoc = async(doc) => {
  const attachment = await useFetchCustom.$fetch(`/Repository/GetAttachment/${doc.id}/0`)
  const mimeType = {
    "PDF": "application/pdf",
    "Word": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "Excel": "application/vnd.ms-excel",
  }
  const arrayBuffer = Buffer.from(attachment, 'base64');
  const blob = new Blob([arrayBuffer], { type: mimeType[doc.type] })
  const url = URL.createObjectURL(blob)

  if (doc.type == 'PDF')
    window.open(url, '_blank')
  else {
    const link = document.createElement('a')
    link.href = url
    link.download = doc.name
    link.click()
    document.body.appendChild(link)
    document.body.removeChild(link)
  }
}
const archiveDoc = async(docId) => {
  try {
    const result = await useFetchCustom.$put(`/Repository/Archive/${docId}/0`)
    if (!result.error) {
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
}
</script>