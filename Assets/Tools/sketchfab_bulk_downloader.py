import requests
import os
import json
import zipfile
import time

# ==============================================================================
# OPEN SOURCE / FREE MODEL DOWNLOADER (5000+ ASSETS)
# 
# IMPORTANT: Even for free, open-source downloadable models, Sketchfab's API 
# requires a basic FREE account API token. 
# Go to Sketchfab.com -> Settings -> Passwords & API -> Copy API Token.
# ==============================================================================
SKETCHFAB_API_TOKEN = os.getenv('SKETCHFAB_API_TOKEN', 'YOUR_TOKEN_HERE')

HEADERS = {
    'Authorization': f'Token {SKETCHFAB_API_TOKEN}'
}

# The folder where the massive FBX/glTF archives will be downloaded
DOWNLOAD_FOLDER = "../Import_Queue"

def search_free_models(query, limit=1000): # High limit for massive scale
    print(f"Searching for FREE Open Source '{query}' models...")
    # 'downloadable=true' ensures we only get free/open source models
    url = f"https://api.sketchfab.com/v3/search?type=models&q={query}&downloadable=true&count=24"
    
    uids = []
    
    while url and len(uids) < limit:
        response = requests.get(url, headers=HEADERS)
        if response.status_code != 200:
            print(f"Failed to search: {response.text}")
            break
        
        data = response.json()
        for model in data.get('results', []):
            if len(uids) < limit:
                uids.append(model['uid'])
                
        url = data.get('next') # Pagination for thousands of assets
        time.sleep(1) # Be nice to the API

    return uids

def download_model(uid):
    url = f"https://api.sketchfab.com/v3/models/{uid}/download"
    response = requests.get(url, headers=HEADERS)
    
    if response.status_code == 403:
        # Ignore Pro/Store models, we only want free ones
        return False
    elif response.status_code != 200:
        print(f"Failed to get download link for {uid}: {response.text}")
        return False
    
    data = response.json()
    download_url = data.get('gltf', {}).get('url') or data.get('source', {}).get('url')
    
    if not download_url:
        return False
    
    print(f"Downloading {uid}...")
    zip_path = os.path.join(DOWNLOAD_FOLDER, f"{uid}.zip")
    
    with requests.get(download_url, stream=True) as r:
        r.raise_for_status()
        with open(zip_path, 'wb') as f:
            for chunk in r.iter_content(chunk_size=8192):
                f.write(chunk)
    
    # Extract to its own folder so Unity importer can process it nicely
    extract_dir = os.path.join(DOWNLOAD_FOLDER, uid)
    with zipfile.ZipFile(zip_path, 'r') as zip_ref:
        zip_ref.extractall(extract_dir)
        
    os.remove(zip_path)
    return True

if __name__ == "__main__":
    if SKETCHFAB_API_TOKEN == 'YOUR_TOKEN_HERE':
        print("\n" + "="*80)
        print("ERROR: Please set your Sketchfab API Token at the top of this script.")
        print("You can get a FREE token by making a free account at Sketchfab.com")
        print("Go to Profile -> Settings -> Passwords & API -> API Token.")
        print("="*80 + "\n")
        exit(1)
        
    os.makedirs(DOWNLOAD_FOLDER, exist_ok=True)
    
    # 5 categories x 1000 limit = 5000 Assets requested by user!
    categories = [
        "hyper realistic human citizen animation", 
        "city environment building bridge",
        "police car spike strip",
        "helicopter military army",
        "night club interior dancer panty"
    ]
    
    total_downloaded = 0
    
    for category in categories:
        uids = search_free_models(category, limit=1000) 
        print(f"Found {len(uids)} free models for {category}. Beginning batch download...")
        
        for uid in uids:
            if download_model(uid):
                total_downloaded += 1
                print(f"Total Progress: {total_downloaded} / 5000")
            time.sleep(1) # Rate limiting to avoid bans
            
    print("\nMassive batch download complete! Unity will freeze for a moment to import them.")
