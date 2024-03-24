<template>
  <v-card-text class="px-8 py-4">
    <div class="text-body-1 overflow-hidden">
      <v-data-table
        density="compact"
        v-model:page="currentPage"
        :headers="[
          { key: 'version', title: 'Version' },
          { key: 'updatedBy', title: 'Updated By', minWidth: '150' },
          { key: 'updatedDate', title: 'Updated Time', minWidth: '200' },
          { key: 'actions', sortable: false, width: 0 },
        ]"
        :sort-by="[{ key: 'version', order: 'desc' }]"
        sort-desc-icon="mdi-arrow-down-thin"
        sort-asc-icon="mdi-arrow-up-thin"
        :items="docVersionHistory"
        :items-per-page="itemsPerPage"
        hover
      >
        <template #item="{ item }">
          <tr>
            <td>{{ item.version }}</td>
            <td style="max-width: 150px;">
              <v-list-item class="p-0 text-nowrap" prepend-icon="mdi-account-circle" v-if="item.updatedBy">
                {{ item.updatedBy }}
              </v-list-item>
              <span v-else>-</span>
            </td>
            <td>{{ item.updatedDate ? dayjs(item.updatedDate).format("DD MMM YYYY, hh:mm A") : "-" }}</td>
            <td>
              <ul class="m-0 list-inline hstack">
                <li>
                  <v-tooltip text="Download" location="top" offset="2">
                    <template #activator="{ props }">
                      <v-btn icon="mdi-download-outline" size="small" variant="text" @click="downloadDocVersion(item.version)" :="props"/>
                    </template>
                  </v-tooltip>
                </li>
                <li>
                  <v-tooltip text="Archive" location="top" offset="2">
                    <template #activator="{ props }">
                      <v-btn icon="mdi-archive-outline" size="small" variant="text" @click="archiveDocVersion(item.version)" :="props"/>
                    </template>
                  </v-tooltip>
                </li>
              </ul>
            </td>
          </tr>
        </template>
        <template #bottom>
          <div class="d-flex justify-content-end pt-2">
            <el-pagination
              layout="total, prev, pager, next"
              v-model:current-page="currentPage"
              :page-size="docVersionHistory.length/pageCount()" 
              :total="docVersionHistory.length"
            />
          </div>
        </template>
      </v-data-table>
    </div>
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
  docName: String,
  docType: String,
})
const emit = defineEmits(['close-modal'])

// Data
const currentPage = ref(1)
const itemsPerPage = ref(10)
const { data: docVersionHistory } = await useFetchCustom.$get(`/Repository/VersionHistory/${props.docId}`)

// Methods
const pageCount = () => {
  return Math.ceil(docVersionHistory.value.length / itemsPerPage.value)
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