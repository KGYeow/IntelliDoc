<template>
  <v-card-text class="px-8 pt-4 pb-0">
    <div class="text-body-1 overflow-hidden">
      <v-data-table
        density="compact"
        v-model:page="currentPage"
        :headers="[
          { key: 'version', title: 'Version' },
          { key: 'modifiedBy', title: 'Modified By', minWidth: '150' },
          { key: 'modifiedDate', title: 'Modified Time', minWidth: '100' },
          { key: 'actions', sortable: false, width: 0 },
        ]"
        :sort-by="[{ key: 'version', order: 'desc' }]"
        sort-desc-icon="mdi-arrow-down-thin"
        sort-asc-icon="mdi-arrow-up-thin"
        :items="docVersionHistory"
        :items-per-page="itemsPerPage"
        :loading="loading"
        hover
      >
        <template #loading>
          <v-skeleton-loader type="table-row@5"/>
        </template>
        <template #item="{ item }">
          <tr>
            <td>{{ item.version }}</td>
            <td style="max-width: 150px;">
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
const itemsPerPage = ref(5)
const { data: docVersionHistory, pending: loading } = await useFetchCustom.$get(`/Repository/VersionHistory/${props.docId}`)

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