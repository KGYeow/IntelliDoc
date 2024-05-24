<template>
  <v-card-text class="px-8 pt-4 pb-0">
    <v-row>
      <v-col cols="9">
        <div class="text-body-1 overflow-hidden">
          <v-data-table
            density="comfortable"
            v-model:page="currentPage"
            :headers="[
              { key: 'name', title: 'Name' },
              { key: 'modifiedBy', title: 'Modified By', width: '140' },
              { key: 'modifiedDate', title: 'Modified Time', width: '140' },
              { key: 'actions', sortable: false, width: 0 },
            ]"
            :sort-by="[{ key: 'name', order: 'asc' }]"
            sort-desc-icon="mdi-arrow-down-thin"
            sort-asc-icon="mdi-arrow-up-thin"
            :items="relatedDocs"
            :items-per-page="itemsPerPage"
            hover
          >
            <template #item="{ item }">
              <tr :class="{ 'bg-background': false }">
                <td style="max-width: 420px;">
                  <v-list-item
                    class="p-0 text-nowrap"
                    :prepend-icon="'text-h5 '+ (item.type == 'PDF' ? 'mdi-file-pdf-box' : 'mdi-file-word-box')"
                  >
                    <span class="row-link" @click="showInformation(item)">
                      <v-tooltip :text="item.name" activator="parent" location="top" offset="2"/>
                      {{ item.name }}
                    </span>
                  </v-list-item>
                </td>
                <td style="max-width: 140px;">
                  <v-list-item class="p-0 text-nowrap" prepend-icon="mdi-account-circle text-h5">
                    <span v-if="item.modifiedBy">
                      <v-tooltip :text="item.modifiedBy" activator="parent" location="top" offset="2"/>
                      {{ item.modifiedBy }}
                    </span>
                    <span class="text-muted fst-italic" v-else>
                      <v-tooltip text="Deleted Account" activator="parent" location="top" offset="2"/>
                      Deleted Account
                    </span>
                  </v-list-item>
                </td>
                <td>
                  <span>
                    <v-tooltip :text="dayjs(item.modifiedDate).format('DD MMM YYYY, hh:mm A')" activator="parent" location="top" offset="2"/>
                    {{ dayjs(item.modifiedDate).format("DD MMM YYYY") }}
                  </span>
                </td>
                <td>
                  <ul class="m-0 list-inline hstack actions">
                    <li>
                      <v-menu width="220" location="left" offset="2">
                        <template #activator="{ props }">
                          <v-btn size="small" variant="text" :="props" icon>
                            <v-icon icon="mdi-dots-vertical"/>
                            <v-tooltip text="More Actions" location="top" offset="2" activator="parent"/>
                          </v-btn>
                        </template>
                        <v-list class="text-body-1" density="compact" elevation="4">
                          <v-list-item prepend-icon="mdi-download-outline" @click="downloadDoc(item)">Download</v-list-item>
                          <v-list-item prepend-icon="mdi-upload-outline" @click="emit('select-doc', ['Update', item])">Update</v-list-item>
                          <v-list-item prepend-icon="mdi-pencil-outline" @click="emit('select-doc', ['Edit', item])">Edit</v-list-item>
                          <v-list-item prepend-icon="mdi-rename-outline" @click="emit('select-doc', ['Rename', item])">Rename</v-list-item>
                          <v-divider class="m-2"/>
                          <v-list-item prepend-icon="mdi-history" @click="emit('select-doc', ['VersionHistory', item])">Version History</v-list-item>
                          <v-divider class="m-2"/>
                          <v-list-item prepend-icon="mdi-archive-outline" @click="archiveDoc(item.id)">Archive</v-list-item>
                        </v-list>
                      </v-menu>
                    </li>
                  </ul>
                </td>
              </tr>
            </template>
            <template #bottom>
              <div class="d-flex justify-content-end pt-2">
                <el-pagination
                  :layout="relatedDocs.length > itemsPerPage ? 'total, prev, pager, next' : 'total'"
                  v-model:current-page="currentPage"
                  :page-size="relatedDocs.length/pageCount()"
                  :total="relatedDocs.length"
                />
              </div>
            </template>
          </v-data-table>
        </div>
      </v-col>
      <div class="mb-7">
        <v-divider vertical/>
      </div>
      <v-col cols="3" style="height: 376px;">
        <el-scrollbar max-height="352px">
          <div class="text-body-1 overflow-hidden">
            <el-empty class="my-8" :image-size="120" description="No selected document information" v-if="selectedDocInfo == null"/>
            <v-row class="pt-3" v-else>
              <v-col>
                <v-row class="pb-3">
                  <v-col class="py-0" cols="12">
                    <v-label class="text-caption fw-bold">Name</v-label>
                  </v-col>
                  <v-col class="py-0" cols="12">
                    <div class="text-body-2">{{ selectedDocInfo.name }}</div>
                  </v-col>
                </v-row>
                <v-row class="pb-3">
                  <v-col class="py-0" cols="12">
                    <v-label class="text-caption fw-bold">Version</v-label>
                  </v-col>
                  <v-col class="py-0" cols="12">
                    <div class="text-body-2">{{ selectedDocInfo.currentVersion }}</div>
                  </v-col>
                </v-row>
                <v-row class="pb-3">
                  <v-col class="py-0" cols="12">
                    <v-label class="text-caption fw-bold">Type</v-label>
                  </v-col>
                  <v-col class="py-0" cols="12">
                    <div class="text-body-2">{{ selectedDocInfo.type }}</div>
                  </v-col>
                </v-row>
                <v-row class="pb-3">
                  <v-col class="py-0" cols="12">
                    <v-label class="text-caption fw-bold">Modified By</v-label>
                  </v-col>
                  <v-col class="py-0" cols="12">
                    <div class="text-body-2">
                      <span v-if="selectedDocInfo.modifiedBy">{{ selectedDocInfo.modifiedBy }}</span>
                      <span class="text-muted fst-italic" v-else>Deleted Account</span>
                    </div>
                  </v-col>
                </v-row>
                <v-row class="pb-3">
                  <v-col class="py-0" cols="12">
                    <v-label class="text-caption fw-bold">Modified Time</v-label>
                  </v-col>
                  <v-col class="py-0" cols="12">
                    <div class="text-body-2">{{ dayjs(selectedDocInfo.modifiedDate).format('DD MMM YYYY, hh:mm A') }}</div>
                  </v-col>
                </v-row>
                <v-row class="pb-3">
                  <v-col class="py-0" cols="12">
                    <v-label class="text-caption fw-bold">Description</v-label>
                  </v-col>
                  <v-col class="py-0" cols="12">
                    <div class="text-body-2">{{ selectedDocInfo.description ?? '-' }}</div>
                  </v-col>
                </v-row>
              </v-col>
            </v-row>
          </div>
        </el-scrollbar>
      </v-col>
    </v-row>
  </v-card-text>
  <v-card-actions class="p-3 justify-content-end">
    <v-btn color="primary" @click="emit('close-modal', false)">Close</v-btn>
  </v-card-actions>
</template>

<script setup>
import { Buffer } from 'buffer'
import dayjs from 'dayjs'

// Properties, Emit & Model
const props = defineProps({
  docId: Number,  
})
const emit = defineEmits(['close-modal','select-doc'])

// Data
const currentPage = ref(1)
const itemsPerPage = ref(6)
const selectedDocInfo = ref(null)
const { data: relatedDocs } = await useFetchCustom.$get(`/Repository/RelatedDocs/${props.docId}`)

// Methods
const pageCount = () => {
  return Math.ceil(relatedDocs.value.length / itemsPerPage.value)
}
const selectDoc = (action, doc) => {
  selectedDocInfo.value = doc

  // if (action == "Rename") {
  //   renameDocModal.value = true
  // }
  // else if (action == "Edit") {
  //   editDocModal.value = true
  // }
  // else if (action == "Update") {
  //   updateAttachmentModal.value = true
  // }
  // else if (action == "VersionHistory") {
  //   versionHistoryModal.value = true
  // }
  // else if (action == "RelatedDoc") {
  //   relatedDocModal.value = true
  // }
  // else {
  //   ElNotification.error({ message: "Undefined action performed" })
  // }
}
const showInformation = (doc) => {
  selectedDocInfo.value = doc
}
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
const downloadDocVersion = async(docVersion) => {
  const attachment = await useFetchCustom.$fetch(`/Repository/GetAttachment/${props.docId}/${docVersion}`)
  const mimeType = {
    "PDF": "application/pdf",
    "Word": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "Excel": "application/vnd.ms-excel",
  }
  const arrayBuffer = Buffer.from(attachment, 'base64');
  const blob = new Blob([arrayBuffer], { type: mimeType[props.docType] })
  const url = URL.createObjectURL(blob)

  if (props.docType == 'PDF')
    window.open(url, '_blank')
  else {
    const link = document.createElement('a')
    link.href = url
    link.download = props.docName
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
const archiveDocVersion = async(docVersion) => {
  try {
    const result = await useFetchCustom.$put(`/Repository/Archive/${props.docId}/${docVersion}`)
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