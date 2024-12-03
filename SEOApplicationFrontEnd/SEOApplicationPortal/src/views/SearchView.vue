<template>
  <div class="custom-container">
    <div class="form-section">
      <div class="header">
        <h2>SEO Google Search URL Ranker</h2>
      </div>
      <form @submit.prevent="onSearch" class="search-form">
        <input
          v-model="searchPhrase"
          type="text"
          class="input-field"
          placeholder="Enter search query"
          required
        />
        <input
          v-model="targetUrl"
          type="text"
          class="input-field"
          placeholder="Enter target URL"
          required
        />
        <button type="submit" class="submit-button">Search</button>
      </form>
      <div v-if="loading" class="loading-message">Loading...</div>
      <div v-if="error" class="error-message">An error occurred. Please try again.</div>
      <div v-if="result === '0'" class="empty-result">No results found.</div>
      <div v-if="result && result !== '0'" class="result-container">
        <h2>Search Result:</h2>
        <p class="result-text">{{ result }}</p>
      </div>
    </div>
    <div v-if="ranks.length > 0" class="table-section">
      <div class="header">
        <h2>All Rankings</h2>
      </div>
      <div class="table-wrapper">
        <table border="1">
          <thead>
            <tr>
              <th>Search Phrase</th>
              <th>URL</th>
              <th>Rank</th>
              <th>Searched On</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(rank, index) in ranks" :key="index">
              <td>{{ rank.searchPhrase }}</td>
              <td>{{ rank.url }}</td>
              <td>{{ rank.rank }}</td>
              <td>{{ formatDate(rank.searchedOn) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  data() {
    return {
      searchPhrase: '',
      targetUrl: '',
      result: null,
      ranks: [],
      loading: false,
      error: false,
    }
  },
  mounted() {
    this.fetchRankings()
  },
  methods: {
    async onSearch() {
      this.loading = true
      this.error = false
      this.result = null

      try {
        const searchResponse = await axios.get('https://localhost:7222/api/SEO/scrape-google', {
          params: {
            searchPhrase: this.searchPhrase,
            targetUrl: this.targetUrl,
          },
        })

        if (String(searchResponse.data) === '0') {
          this.result = '0'
        } else {
          this.result = searchResponse.data
        }
      } catch (error) {
        this.error = true
        console.error('Error during search:', error)
      } finally {
        this.loading = false
      }
      this.fetchRankings()
    },

    async fetchRankings() {
      try {
        const rankingsResponse = await axios.get('https://localhost:7222/api/SEO/get-rankings')
        this.ranks = rankingsResponse.data.ranks || []
        this.ranks.sort((a, b) => {
          const dateA = new Date(a.searchedOn)
          const dateB = new Date(b.searchedOn)
          return dateB - dateA
        })
      } catch (error) {
        console.error('Error fetching rankings:', error)
      }
    },

    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleString()
    },
  },
}
</script>

<style scoped>
.custom-container {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  padding: 2rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  font-family: Arial, sans-serif;
  min-height: 60vh;
  width: 80vw;
}

.header {
  text-align: center;
  margin-bottom: 2rem;
}

.header h1 {
  font-size: 2.5rem;
  color: #333;
}

.form-section {
  width: 48%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.table-section {
  width: 48%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.search-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.input-field {
  padding: 1rem;
  font-size: 1rem;
  border: 1px solid #ddd;
  border-radius: 0.4rem;
  outline: none;
  transition: all 0.3s ease;
}

.input-field:focus {
  border-color: #007bff;
}

.submit-button {
  padding: 1.2rem;
  font-size: 1rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 0.4rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.submit-button:hover {
  background-color: #0056b3;
}

.submit-button:active {
  background-color: #004085;
}

.loading-message {
  color: #007bff;
  font-size: 1.5rem;
  font-weight: bold;
  margin-top: 20px;
}

.error-message {
  color: red;
  font-size: 1.5rem;
  font-weight: bold;
  margin-top: 20px;
}

.empty-result {
  color: red;
  font-size: 1.5rem;
  font-weight: bold;
  margin-top: 20px;
}

.result-container {
  margin-top: 20px;
  background-color: #f1f1f1;
  padding: 15px;
  border-radius: 4px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.result-text {
  font-size: 1.1rem;
  color: #333;
  word-wrap: break-word;
}

.table-container {
  width: 100%;
  margin-top: 40px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.table-wrapper {
  width: 80%;
  max-height: 300px;
  overflow-y: auto;
  margin-top: 1rem;
}

table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

th,
td {
  padding: 10px;
  text-align: left;
  border: 1px solid #ddd;
}

th {
  background-color: #f2f2f2;
}

tr:nth-child(even) {
  background-color: #f9f9f9;
}

tr:hover {
  background-color: #e9e9e9;
}
</style>
