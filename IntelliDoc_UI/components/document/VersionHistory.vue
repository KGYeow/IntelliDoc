<template>
  <v-card-text class="px-8 py-4">
    <div class="text-body-1">
      <v-data-table
        density="compact"
        v-model:page="currentPage"
        :headers="[
          { key: 'version', title: 'Version' },
          { key: 'updatedBy', title: 'Updated By' },
          { key: 'updatedDate', title: 'Updated Time' },
          { key: 'type', title: 'Type' },
          { key: 'actions', sortable: false, width: 0 },
        ]"
        :items="docVersionHistory"
        :items-per-page="itemsPerPage"
        hover
      >
        <template #item="{ item }">
          <tr>
            <td>{{ item.version }}</td>
            <td>{{ item.updatedBy ?? "-" }}</td>
            <td>{{ item.updatedDate ? dayjs(item.updatedDate).format("DD MMM YYYY, hh:mm A") : "-" }}</td>
            <td>{{ item.type }}</td>
            <td>
              <ul class="m-0 list-inline hstack">
                <li>
                  <v-tooltip text="Download" activator="parent" location="top" offset="2"/>
                  <v-btn icon="mdi-download-outline" size="small" variant="text" @click="downloadDoc(item.version)"/>
                </li>
                <li>
                  <v-tooltip text="Archive" activator="parent" location="top" offset="2"/>
                  <v-btn icon="mdi-archive-outline" size="small" variant="text" @click="archiveDoc(item.id)"/>
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
})
const emit = defineEmits(['close-modal'])

// Data
const currentPage = ref(1)
const itemsPerPage = ref(10)
const { data: docVersionHistory } = await fetchData.$get(`/Repository/VersionHistory/${props.docId}`)

// Methods
const pageCount = () => {
  return Math.ceil(docVersionHistory.value.length / itemsPerPage.value)
}
const downloadDoc = async(docVersion) => {
  const { data } = await fetchData.$get(`/Repository/GetAttachment/${props.docId}/${docVersion}`)
  const mimeType = {
    "PDF": "application/pdf",
    "Word": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "Excel": "application/vnd.ms-excel",
  }
  const arrayBuffer = Buffer.from(data.value.attachment, 'base64');
  const blob = new Blob([arrayBuffer], { type: mimeType[data.value.type] })
  const url = URL.createObjectURL(blob)

  if (data.value.type == 'PDF')
    window.open(url, '_blank')
  else {
    const link = document.createElement('a')
    link.href = url
    link.download = docName
    link.click()
    document.body.removeChild(link)
  }
}
</script>