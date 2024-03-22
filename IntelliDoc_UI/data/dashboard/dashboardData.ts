const { data } = await fetchData.$get("/Dashboard/DashboardData")

const docCategory = data.value.docCategory
const storedDocNum = data.value.storedDocNum
const archivedDocNum = data.value.archivedDocNum
const yAxisMax = data.value.yAxisMax
const totalStoredDoc = data.value.totalStoredDoc
const top3CategoriesName = data.value.top3CategoriesName
const top3CategoriesFrequency = data.value.top3CategoriesFrequency

export { docCategory, storedDocNum, archivedDocNum, yAxisMax, totalStoredDoc, top3CategoriesName, top3CategoriesFrequency }